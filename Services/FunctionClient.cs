using System.Text.Json;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Services;

/// <summary>
/// Talks to the GiftOfTheGivers Azure Function App (see the Function project).
/// Every call degrades gracefully: when the function app is not running
/// (e.g. offline development) the web app keeps working without it.
/// </summary>
public class FunctionClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FunctionClient> _logger;
    private readonly string _baseUrl;

    public FunctionClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FunctionClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("functions");
        _logger = logger;
        _baseUrl = configuration["Functions:BaseUrl"] ?? "http://localhost:7071";
    }

    /// <summary>
    /// Asks the TaxCertificate function to generate a dummy tax certificate
    /// for a completed donation. Returns null when the function is unreachable.
    /// </summary>
    public async Task<TaxCertificate?> RequestTaxCertificateAsync(Donation donation)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/taxcertificate", new TaxCertificateRequest
            {
                DonationId = donation.Id,
                Reference = donation.Reference,
                DonorName = donation.IsAnonymous ? null : donation.DonorName,
                Amount = donation.Amount,
                Currency = donation.Currency,
                DonationType = donation.DonationType,
                CreatedAtUtc = donation.CreatedAt
            }, timeout.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("TaxCertificate function returned {StatusCode}.", response.StatusCode);
                return null;
            }

            var certificate = await response.Content.ReadFromJsonAsync<TaxCertificate>(cancellationToken: timeout.Token);
            _logger.LogInformation("Tax certificate {Number} received from function app.", certificate?.CertificateNumber);
            return certificate;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "TaxCertificate function unreachable - falling back to local certificate generation.");
            return null;
        }
    }

    /// <summary>
    /// Forwards a posted project update to the LogProjectUpdate function,
    /// which stores it in Azure Blob Storage. Failures are logged and ignored.
    /// </summary>
    public async Task LogProjectUpdateAsync(Project project, ProjectUpdate update)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var payload = new
            {
                projectId = project.Id,
                projectTitle = project.Title,
                updateId = update.Id,
                title = update.Title,
                body = update.Body,
                postedBy = update.PostedBy,
                createdAtUtc = update.CreatedAt
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/logprojectupdate", payload, timeout.Token);
            _logger.LogInformation("Project update forwarded to LogProjectUpdate function ({StatusCode}).", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LogProjectUpdate function unreachable - skipping Azure Storage log.");
        }
    }
}
