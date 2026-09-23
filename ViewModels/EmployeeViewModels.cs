using System.ComponentModel.DataAnnotations;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.ViewModels;

public class EmployeeDashboardViewModel
{
    public int ActiveProjects { get; set; }

    public int VolunteerCount { get; set; }

    public int PendingVolunteers { get; set; }

    public int UpdateCount { get; set; }

    public int DonationCount { get; set; }

    public decimal TotalDonated { get; set; }

    public List<Project> OngoingProjects { get; set; } = new();

    public List<Donation> RecentDonations { get; set; } = new();

    public List<Volunteer> RecentVolunteers { get; set; } = new();
}

public class VolunteerManagementViewModel
{
    public int TotalCount { get; set; }

    public int PendingCount { get; set; }

    public int ApprovedCount { get; set; }

    public List<Volunteer> Volunteers { get; set; } = new();
}

public class PostUpdateViewModel
{
    [Required(ErrorMessage = "Please select a relief project.")]
    [Display(Name = "Select Project")]
    public int? ProjectId { get; set; }

    [Required(ErrorMessage = "Please enter an update title.")]
    [StringLength(120)]
    [Display(Name = "Update Title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional: also updates the project's progress bar.</summary>
    [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
    [Display(Name = "Project Progress (%)")]
    public int? ProgressPercent { get; set; }

    [Required(ErrorMessage = "Please enter an update description.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "The description must be between 10 and 2000 characters.")]
    [Display(Name = "Update Description")]
    public string Body { get; set; } = string.Empty;
}
