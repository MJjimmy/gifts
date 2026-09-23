namespace GiftOfTheGivers.Models;

/// <summary>
/// A humanitarian relief project (e.g. flood relief, food parcels, medical aid).
/// </summary>
public class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>Where the project operates, e.g. "Gauteng, South Africa".</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Short summary shown on the project card.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Full description shown on the details page.</summary>
    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = "Relief";

    /// <summary>Ongoing, Completed or Planned.</summary>
    public string Status { get; set; } = "Ongoing";

    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>Implementation progress 0-100, shown on progress bars.</summary>
    public int ProgressPercent { get; set; }

    public int VolunteerCount { get; set; }

    public int ReliefPackagesDelivered { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>One objective per line (displayed as a checklist).</summary>
    public List<string> Objectives { get; set; } = new();

    public List<ProjectUpdate> Updates { get; set; } = new();
}
