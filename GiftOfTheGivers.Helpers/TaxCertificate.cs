namespace GiftOfTheGivers.Helpers;

/// <summary>
/// Data for a placeholder donation tax certificate, shared between the
/// web app and the Azure Function that generates certificates.
/// </summary>
public class TaxCertificate
{
    public string CertificateNumber { get; set; } = string.Empty;

    public string DonationReference { get; set; } = string.Empty;

    public string DonorName { get; set; } = "Anonymous Donor";

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "ZAR";

    public string DonationType { get; set; } = "OneTime";

    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Formatted amount including the currency symbol, e.g. "R500.00".</summary>
    public string FormattedAmount => $"{DonationCalculator.CurrencySymbol(Currency)}{Amount:N2}";
}

/// <summary>Request payload sent to the tax certificate function.</summary>
public class TaxCertificateRequest
{
    public int DonationId { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string? DonorName { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "ZAR";

    public string DonationType { get; set; } = "OneTime";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
