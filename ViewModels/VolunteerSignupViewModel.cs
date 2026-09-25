using GiftOfTheGivers.Validation;
using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.ViewModels;

/// <summary>
/// Form model for the volunteer registration page.
/// </summary>
public class VolunteerSignupViewModel
{
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(30)]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Skills { get; set; }

    [Required(ErrorMessage = "Please select your availability.")]
    [StringLength(50)]
    public string Availability { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select an area of interest.")]
    [StringLength(50)]
    [Display(Name = "Area of Interest")]
    public string AreaOfInterest { get; set; } = string.Empty;

    [MustBeTrue(ErrorMessage = "Please agree to be contacted regarding volunteer opportunities.")]
    [Display(Name = "Agreement")]
    public bool Agreement { get; set; }
}
