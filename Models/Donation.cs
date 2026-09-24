using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models;

public class Donation
{
    public int Id { get; set; }

    /// <summary>Public reference shown to the donor, e.g. GTG-20260924-A1B2C3.</summary>
    public string Reference { get; set; } = string.Empty;

    /// <summary>"OneTime" or "Recurring".</summary>
    public string DonationType { get; set; } = "OneTime";

    [Range(1, 1_000_000)]
    public decimal Amount { get; set; }

    /// <summary>ISO currency code, e.g. ZAR.</summary>
    public string Currency { get; set; } = "ZAR";

    /// <summary>Null when donating anonymously.</summary>
    public string? DonorName { get; set; }

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool IsAnonymous { get; set; }

    /// <summary>Optional project the donation is directed at.</summary>
    public int? ProjectId { get; set; }

    public Project? Project { get; set; }

    public string? Message { get; set; }

    /// <summary>Received, Confirmed or Cancelled.</summary>
    public string Status { get; set; } = "Received";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
