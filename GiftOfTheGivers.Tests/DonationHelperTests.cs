using GiftOfTheGivers.Helpers;
using Xunit;

namespace GiftOfTheGivers.Tests;

public class DonationReferenceGeneratorTests
{
    [Fact]
    public void Create_ReturnsPrefixedReference()
    {
        var reference = DonationReferenceGenerator.Create(new DateTime(2026, 9, 24, 10, 0, 0, DateTimeKind.Utc));

        Assert.StartsWith("GTG-20260924-", reference);
    }

    [Fact]
    public void Create_ReturnsCorrectTotalLength()
    {
        var reference = DonationReferenceGenerator.Create(DateTime.UtcNow);

        // GTG- (4) + yyyyMMdd (8) + - (1) + 6 chars = 19
        Assert.Equal(19, reference.Length);
    }

    [Fact]
    public void Create_DoesNotUseAmbiguousCharacters()
    {
        for (var i = 0; i < 50; i++)
        {
            var reference = DonationReferenceGenerator.Create(DateTime.UtcNow);
            var suffix = reference[^6..];

            Assert.DoesNotContain("O", suffix);
            Assert.DoesNotContain("0", suffix);
            Assert.DoesNotContain("I", suffix);
            Assert.DoesNotContain("1", suffix);
        }
    }
}

public class TaxCertificateNumberFormatterTests
{
    [Fact]
    public void Format_PadsDonationIdToFourDigits()
    {
        var number = TaxCertificateNumberFormatter.Format(7, new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal("GOTG-2026-0007", number);
    }

    [Fact]
    public void Format_UsesIssueYear()
    {
        var number = TaxCertificateNumberFormatter.Format(1234, new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal("GOTG-2027-1234", number);
    }
}

public class DonationCalculatorTests
{
    [Fact]
    public void Total_SumsAllAmounts()
    {
        var total = DonationCalculator.Total(new[] { 100m, 250.50m, 1000m });

        Assert.Equal(1350.50m, total);
    }

    [Fact]
    public void Total_EmptyListIsZero()
    {
        Assert.Equal(0m, DonationCalculator.Total(Array.Empty<decimal>()));
    }

    [Theory]
    [InlineData("ZAR", "R")]
    [InlineData("USD", "$")]
    [InlineData("EUR", "\u20AC")]
    [InlineData("GBP", "\u00A3")]
    [InlineData("", "R")]
    public void CurrencySymbol_MapsKnownCodes(string code, string expected)
    {
        Assert.Equal(expected, DonationCalculator.CurrencySymbol(code));
    }

    [Fact]
    public void FormatAmount_IncludesSymbolAndThousandsSeparator()
    {
        var formatted = DonationCalculator.FormatAmount(1250m, "ZAR");

        Assert.StartsWith("R", formatted);
        Assert.Contains("1", formatted);
        Assert.Contains("250.00", formatted);
    }
}
