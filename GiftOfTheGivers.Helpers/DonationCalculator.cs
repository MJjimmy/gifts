namespace GiftOfTheGivers.Helpers;

/// <summary>
/// Simple donation maths and currency formatting used across the web app and functions.
/// </summary>
public static class DonationCalculator
{
    /// <summary>Sums a list of donation amounts.</summary>
    public static decimal Total(IEnumerable<decimal> amounts)
    {
        ArgumentNullException.ThrowIfNull(amounts);
        return amounts.Sum();
    }

    /// <summary>Returns the currency symbol for an ISO code (defaults to ZAR "R").</summary>
    public static string CurrencySymbol(string currencyCode)
    {
        return currencyCode?.Trim().ToUpperInvariant() switch
        {
            "USD" => "$",
            "EUR" => "\u20AC",
            "GBP" => "\u00A3",
            _ => "R"
        };
    }

    /// <summary>Formats an amount with its currency symbol, e.g. "R1 250.00".</summary>
    public static string FormatAmount(decimal amount, string currencyCode)
    {
        return $"{CurrencySymbol(currencyCode)}{amount.ToString("N2")}"; 
    }
}
