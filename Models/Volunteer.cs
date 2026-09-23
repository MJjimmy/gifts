namespace GiftOfTheGivers.Models;

public class Volunteer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    /// <summary>City or region, e.g. "Pretoria, Gauteng".</summary>
    public string? Location { get; set; }

    /// <summary>Skills the volunteer can offer, e.g. driving, cooking, medical.</summary>
    public string? Skills { get; set; }

    /// <summary>Availability, e.g. "Weekends", "Weekdays".</summary>
    public string Availability { get; set; } = "Weekends";

    /// <summary>Why they want to volunteer (free text).</summary>
    public string? Motivation { get; set; }

    /// <summary>Pending, Approved or Declined.</summary>
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
