using Microsoft.Extensions.DependencyInjection;

namespace AuthPlus.Identity.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPolicies(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole",
                p => p.RequireRole("Admin"));

            options.AddPolicy("RequireUserRole",
                p => p.RequireRole("User"));

            options.AddPolicy("RequireAdminOrUserRole",
                p => p.RequireRole("Admin", "User"));
        });

        return services;
    }
}