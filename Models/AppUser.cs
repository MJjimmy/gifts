using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Models;

/// <summary>Application user. UserName and Email are the same (email login).</summary>
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
