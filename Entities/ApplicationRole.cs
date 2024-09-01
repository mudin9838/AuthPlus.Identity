using Microsoft.AspNetCore.Identity;

namespace AuthPlus.Identity.Entities;
public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}