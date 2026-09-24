namespace GiftOfTheGivers.Helpers;

/// <summary>
/// Generates public donation references, e.g. GTG-20260924-K7QX2M.
/// </summary>
public static class DonationReferenceGenerator
{
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    /// <summary>Creates a donation reference such as GTG-20260924-K7QX2M.</summary>
    public static string Create(DateTime utcNow)
    {
        var suffix = new string(Enumerable.Range(0, 6)
            .Select(_ => Chars[Random.Shared.Next(Chars.Length)])
            .ToArray());

        return $"GTG-{utcNow:yyyyMMdd}-{suffix}";
    }
}
