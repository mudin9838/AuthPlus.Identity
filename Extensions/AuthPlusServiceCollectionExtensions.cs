using AuthPlus.Identity.Data;
using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Entities;
using AuthPlus.Identity.Helpers;
using AuthPlus.Identity.Interfaces;
using AuthPlus.Identity.Services;
using AuthPlus.Identity.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        // ---------------- DATABASE ----------------
        services.AddDbContext<AuthDbContext>(db =>
        {
            options.DatabaseProvider.Configure(db, configuration);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        // ---------------- JWT ----------------
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JwtSettings missing");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

        services.AddSingleton(new JwtHelper(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience));

        services.AddPolicies();

        // ---------------- EMAIL ----------------
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService>(sp =>
        {
            var emailSettings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;
            return new EmailService(
                emailSettings.SmtpServer,
                emailSettings.SmtpPort,
                emailSettings.SmtpUser,
                emailSettings.SmtpPassword
            );
        });

        // ---------------- SERVICES ----------------
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        // ---------------- VALIDATORS ----------------
        // Default validators (can be overridden in UI)
        TryAddTransientIfNotRegistered<IBaseValidator<LoginDto>, LoginDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<RegisterDto>, RegisterDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<ResetPasswordDto>, ResetPasswordDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<UserDto>, UserDtoValidator>(services);

        return services;
    }

    // Helper: register a service only if it's not already registered
    private static void TryAddTransientIfNotRegistered<TService, TImplementation>(IServiceCollection services)
        where TImplementation : class, TService
        where TService : class
    {
        if (!services.Any(sd => sd.ServiceType == typeof(TService)))
        {
            services.AddTransient<TService, TImplementation>();
        }
    }
}
