using Microsoft.Extensions.DependencyInjection;

namespace AuthPlus.Identity.Extensions;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminOrUserRole", policy =>
               policy.RequireRole(RoleConstants.AdminRole, RoleConstants.UserRole));

            options.AddPolicy("RequireAdminRole", policy =>
               policy.RequireRole(RoleConstants.AdminRole));

            options.AddPolicy("RequireUserRole", policy =>
                policy.RequireRole(RoleConstants.UserRole));
        });
    }
}
