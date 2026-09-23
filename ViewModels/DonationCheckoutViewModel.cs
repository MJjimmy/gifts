using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.ViewModels;

/// <summary>
/// Form model for the donation checkout page.
/// </summary>
public class DonationCheckoutViewModel
{
    [Required(ErrorMessage = "Please choose a donation type.")]
    [StringLength(20)]
    public string DonationType { get; set; } = "OneTime";

    [Required(ErrorMessage = "Please select a currency.")]
    [StringLength(3)]
    public string Currency { get; set; } = "ZAR";

    [Required(ErrorMessage = "Please enter a donation amount.")]
    [Range(1, 1_000_000, ErrorMessage = "The amount must be between 1 and 1 000 000.")]
    [Display(Name = "Donation Amount")]
    public decimal Amount { get; set; }

    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string? DonorName { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(30)]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Display(Name = "Donate anonymously")]
    public bool IsAnonymous { get; set; }

    /// <summary>Optional project to support. Null = General Relief Fund.</summary>
    public int? ProjectId { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }
}
