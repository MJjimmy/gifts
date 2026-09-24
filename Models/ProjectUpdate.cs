namespace GiftOfTheGivers.Models;

/// <summary>
/// A progress update posted by an employee for a relief project.
/// </summary>
public class ProjectUpdate
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project? Project { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    /// <summary>Display name of the employee who posted the update.</summary>
    public string PostedBy { get; set; } = "Employee";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
