using AuthPlus.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthPlus.Identity.Data;

public class AuthDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
}
