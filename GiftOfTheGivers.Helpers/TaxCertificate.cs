namespace GiftOfTheGivers.Helpers;

/// <summary>
/// Data for a placeholder donation tax certificate, shared between the
/// web app and the Azure Function that generates certificates.
/// </summary>
public class TaxCertificate
{
    /// <summary>Formatted certificate number, e.g. GOTG-2026-0001.</summary>
    public string CertificateNumber { get; set; } = string.Empty;

    /// <summary>Public donation reference, e.g. GTG-20260924-K7QX2M.</summary>
    public string DonationReference { get; set; } = string.Empty;

    /// <summary>Display name of the donor (or "Anonymous Donor").</summary>
    public string DonorName { get; set; } = "Anonymous Donor";

    /// <summary>Donated amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>ISO currency code of the donation.</summary>
    public string Currency { get; set; } = "ZAR";

    /// <summary>"OneTime" or "Recurring".</summary>
    public string DonationType { get; set; } = "OneTime";

    /// <summary>When the certificate was issued (UTC).</summary>
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
