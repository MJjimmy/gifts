using GiftOfTheGivers.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions;

/// <summary>
/// Generates a dummy donation tax certificate.
///
/// Called by the Gift of the Givers web app after a donor completes the donation form:
///   POST /api/taxcertificate  (JSON body)  -> JSON certificate
///   GET  /api/taxcertificate?reference=...&amp;amount=...  -> simple HTML certificate
///
/// All input is validated; invalid requests receive a 400 response with a reason.
/// </summary>
public class TaxCertificateFunction
{
    private const decimal MaxAmount = 1_000_000m;

    private readonly ILogger<TaxCertificateFunction> _logger;

    public TaxCertificateFunction(ILogger<TaxCertificateFunction> logger)
    {
        _logger = logger;
    }

    [Function("TaxCertificate")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData request)
    {
        _logger.LogInformation("TaxCertificate function received a {Method} request.", request.Method);

        return request.Method == "GET"
            ? await HandleBrowserRequestAsync(request)
            : await HandleApiRequestAsync(request);
    }

    private async Task<HttpResponseData> HandleApiRequestAsync(HttpRequestData request)
    {
        TaxCertificateRequest? payload;

        try
        {
            payload = await request.ReadFromJsonAsync<TaxCertificateRequest>();
        }
        catch
        {
            _logger.LogWarning("TaxCertificate: request rejected - payload is not valid JSON.");
            return await BadRequestAsync(request, "Invalid JSON payload.");
        }

        if (payload is null || string.IsNullOrWhiteSpace(payload.Reference))
        {
            _logger.LogWarning("TaxCertificate: request rejected - donation reference is missing.");
            return await BadRequestAsync(request, "Reference is required.");
        }

        if (payload.Amount <= 0)
        {
            _logger.LogWarning("TaxCertificate: request rejected for {Reference} - amount must be positive.",
                payload.Reference);
            return await BadRequestAsync(request, "Amount must be greater than zero.");
        }

        if (payload.Amount > MaxAmount)
        {
            _logger.LogWarning("TaxCertificate: request rejected for {Reference} - amount exceeds the limit.",
                payload.Reference);
            return await BadRequestAsync(request, $"Amount must not exceed {MaxAmount}.");
        }

        if (!string.IsNullOrWhiteSpace(payload.Currency) && payload.Currency.Trim().Length != 3)
        {
            _logger.LogWarning("TaxCertificate: request rejected for {Reference} - invalid currency code.",
                payload.Reference);
            return await BadRequestAsync(request, "Currency must be a 3-letter ISO code (e.g. ZAR).");
        }

        var certificate = new TaxCertificate
        {
            CertificateNumber = TaxCertificateNumberFormatter.Format(payload.DonationId, payload.CreatedAtUtc),
            DonationReference = payload.Reference.Trim(),
            DonorName = string.IsNullOrWhiteSpace(payload.DonorName) ? "Anonymous Donor" : payload.DonorName!.Trim(),
            Amount = payload.Amount,
            Currency = string.IsNullOrWhiteSpace(payload.Currency) ? "ZAR" : payload.Currency.Trim().ToUpperInvariant(),
            DonationType = string.IsNullOrWhiteSpace(payload.DonationType) ? "OneTime" : payload.DonationType,
            IssuedAtUtc = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Issued tax certificate {CertificateNumber} for donation {Reference} ({Amount} {Currency}).",
            certificate.CertificateNumber, certificate.DonationReference, certificate.Amount, certificate.Currency);

        var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(certificate);
        return response;
    }

    private static async Task<HttpResponseData> HandleBrowserRequestAsync(HttpRequestData request)
    {
        var query = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
        var reference = query["reference"];
        var amountText = query["amount"];

        if (string.IsNullOrWhiteSpace(reference))
        {
            return await BadRequestAsync(request, "Query parameter 'reference' is required.");
        }

        if (!decimal.TryParse(amountText, out var amount) || amount <= 0)
        {
            return await BadRequestAsync(request, "Query parameter 'amount' must be a positive number.");
        }

        var donor = string.IsNullOrWhiteSpace(query["donor"]) ? "Anonymous Donor" : query["donor"]!;
        var currency = string.IsNullOrWhiteSpace(query["currency"]) ? "ZAR" : query["currency"]!;

        var certificate = new TaxCertificate
        {
            CertificateNumber = TaxCertificateNumberFormatter.Format(0, DateTime.UtcNow),
            DonationReference = reference,
            DonorName = donor,
            Amount = amount,
            Currency = currency
        };

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head><title>Donation Tax Certificate</title></head>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 40px auto; padding: 24px; border: 2px solid #2e7d32; border-radius: 8px;">
                <p style="letter-spacing: 3px; color: #2e7d32;"><strong>GIFT OF THE GIVERS</strong></p>
                <h1 style="font-size: 22px;">DONATION TAX CERTIFICATE</h1>
                <p><em>Placeholder certificate generated by the TaxCertificate Azure Function.</em></p>
                <table cellpadding="8">
                    <tr><td><strong>Certificate Number</strong></td><td>{certificate.CertificateNumber}</td></tr>
                    <tr><td><strong>Donation Reference</strong></td><td>{certificate.DonationReference}</td></tr>
                    <tr><td><strong>Donor</strong></td><td>{certificate.DonorName}</td></tr>
                    <tr><td><strong>Amount</strong></td><td>{certificate.FormattedAmount}</td></tr>
                    <tr><td><strong>Issued (UTC)</strong></td><td>{certificate.IssuedAtUtc:dd MMM yyyy HH:mm}</td></tr>
                </table>
                <p style="color: #888; font-size: 12px;">Prototype only - no real tax document is issued.</p>
            </body>
            </html>
            """;

        var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        await response.WriteStringAsync(html);
        return response;
    }

    private static async Task<HttpResponseData> BadRequestAsync(HttpRequestData request, string message)
    {
        var response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}
