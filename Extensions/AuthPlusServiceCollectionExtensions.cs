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
using System.Security.Claims;
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
        // FIRST: Load JWT settings BEFORE using them
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings missing from configuration.");
        }

        // Validate required JWT settings
        if (string.IsNullOrEmpty(jwtSettings.SecretKey))
            throw new ArgumentException("JwtSettings.SecretKey is required");
        if (string.IsNullOrEmpty(jwtSettings.Issuer))
            throw new ArgumentException("JwtSettings.Issuer is required");
        if (string.IsNullOrEmpty(jwtSettings.Audience))
            throw new ArgumentException("JwtSettings.Audience is required");

        // SECOND: Configure Authentication with JWT Bearer
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                // IMPORTANT: Set claim types
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name,

                // Allow some clock skew
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            // Add debug events
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    Console.WriteLine($"Token received: {context.Token?.Substring(0, Math.Min(20, context.Token?.Length ?? 0))}...");
                    return Task.CompletedTask;
                }
            };
        });

        // THIRD: Register JwtHelper with the SAME settings
        services.AddSingleton<JwtHelper>(sp =>
            new JwtHelper(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience));

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
        TryAddTransientIfNotRegistered<IBaseValidator<LoginDto>, LoginDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<RegisterDto>, RegisterDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<ResetPasswordDto>, ResetPasswordDtoValidator>(services);
        TryAddTransientIfNotRegistered<IBaseValidator<UserDto>, UserDtoValidator>(services);

        return services;
    }

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