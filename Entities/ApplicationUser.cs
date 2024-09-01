using Microsoft.AspNetCore.Identity;

namespace AuthPlus.Identity.Entities;
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }
}