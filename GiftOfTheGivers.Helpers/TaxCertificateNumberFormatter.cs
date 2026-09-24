namespace GiftOfTheGivers.Helpers;

/// <summary>
/// Formats placeholder donation tax certificate numbers, e.g. GOTG-2026-0001.
/// </summary>
public static class TaxCertificateNumberFormatter
{
    /// <summary>Formats a certificate number from the donation id and issue date.</summary>
    public static string Format(int donationId, DateTime issueDateUtc)
    {
        return $"GOTG-{issueDateUtc.Year}-{Math.Max(0, donationId):D4}";
    }
}
