using AuthPlus.Identity.Data;
using AuthPlus.Identity.Entities;
using AuthPlus.Identity.Helpers;
using AuthPlus.Identity.Interfaces;
using AuthPlus.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthPlus.Identity.Extensions;

public static class AuthPlusServiceCollectionExtensions
{
    public static IServiceCollection AddAuthPlusIdentity(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AuthPlusOptions> setup)
    {
        var options = new AuthPlusOptions();
        setup(options);

        if (options.DatabaseProvider is null)
            throw new InvalidOperationException(
                "Database provider not configured. Call UseSqlServer(), UsePostgres(), or UseMySql().");

        services.AddDbContext<AuthDbContext>(db =>
        {
            options.DatabaseProvider.Configure(db, configuration);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddJwt(configuration);
        services.AddPolicies();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
