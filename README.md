# AuthPlus.Identity

AuthPlus.Identity is a .NET 8 class library designed to simplify user authentication and authorization management.

## Features

- User Registration and Login
- User Management
- Password Reset
- Role Management
- JWT-Based Authentication
- Configurable Email Service
- Extendable

## Getting Started

### Prerequisites

- .NET 8 SDK
- NuGet Package Manager

### Installation

Add the AuthPlus.Identity package to your .NET 8 project using NuGet:

````bash
dotnet add package AuthPlus.Identity

### # AuthPlus.Identity

AuthPlus.Identity is a .NET 8 class library designed to simplify user authentication and authorization management.

## Features

- User Registration and Login
- User Management
- Password Reset
- Role Management
- JWT-Based Authentication
- Configurable Email Service
- Extendable


## Getting Started

### Prerequisites

- .NET 8 SDK
- NuGet Package Manager

### Installation

Add the AuthPlus.Identity package to your .NET 8 project using NuGet:

```bash
dotnet add package AuthPlus.Identity


# Configuration
### Add configuration settings to your appsettings.json file:

{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpirationMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@example.com",
    "SmtpPassword": "your-email-password",
    "BaseUrl": "your base url"
  }

//you can add your external service provider by inherit IExternalAuthProvider e.g.
FacebookAuthProvider.cs:
public class FacebookAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;

    public FacebookAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalUserInfo> AuthenticateAsync(string accessToken)
    {
        // Call Facebook's API to get user info
        var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?access_token={accessToken}&fields=id,name,email,picture");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var userData = JObject.Parse(content);

        // Parse the response
        var email = userData["email"]?.ToString();
        var name = userData["name"]?.ToString();
        var profilePictureUrl = userData["picture"]?["data"]?["url"]?.ToString();

        return new ExternalUserInfo
        {
            Email = email,
            Name = name,
            ProfilePictureUrl = profilePictureUrl
        };
    }
}
 //then register in startp/program.cs
 // Register custom external authentication provider (Facebook)
                services.AddHttpClient<FacebookAuthProvider>();



   //Use Authorization policies your controllers or actions:
     By default, the package includes the following authorization policies:

     RequireAdminRole: Requires the user to have the "Admin" role.
     RequireUserRole: Requires the user to have the "User" role.

           [ApiController]
           [Route("api/[controller]")]
           public class SomeController : ControllerBase
           {
               [Authorize(Policy = "RequireAdminRole")]
               [HttpGet("admin")]
               public IActionResult GetAdminData()
               {
               return Ok("Admin data.");
               }

   //then register in startup/program.cs like below

'


## Setting Up
### In your Program.cs, configure the services and middleware:

var builder = WebApplication.CreateBuilder(args);

// Register external authentication providers
builder.Services.AddHttpClient<GoogleAuthProvider>();
builder.Services.AddHttpClient<MicrosoftAuthProvider>();
builder.Services.AddHttpClient<LinkedInAuthProvider>();
// AuthService and external providers
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExternalAuthProvider, GoogleAuthProvider>();
builder.Services.AddScoped<IExternalAuthProvider, MicrosoftAuthProvider>();
builder.Services.AddScoped<IExternalAuthProvider, LinkedInAuthProvider>()
// Configure EmailSettings
var emailSettingsSection = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSettingsSection);

// Register EmailService with EmailSettings
builder.Services.AddSingleton<IEmailService>(serviceProvider =>
{
    var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
    return new EmailService(emailSettings.SmtpServer, emailSettings.SmtpPort, emailSettings.SmtpUser, emailSettings.SmtpPassword);
});

// Configure JwtHelper
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
builder.Services.AddSingleton<JwtHelper>(serviceProvider =>
{
    var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new JwtHelper(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience);
});
// Register default validators
services.AddTransient<IBaseValidator<LoginDto>, LoginDtoValidator>();
services.AddTransient<IBaseValidator<RegisterDto>, RegisterDtoValidator>();
services.AddTransient<IBaseValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();
services.AddTransient<IBaseValidator<UserDto>, UserDtoValidator>();
//// A override or add  own validators
//services.AddTransient(typeof(IBaseValidator<>), typeof(BaseValidator<>));


//you can provide  own validator by overriding the existing on create a new class
//public class CustomRegisterDtoValidator : RegisterDtoValidator
//{
  // public CustomRegisterDtoValidator()
  // {
       // Add or override rules here
      // RuleFor(x => x.Password).Must(password => password.Contains("@")).WithMessage("Password must contain '@'.");
   //}
//}

////services.AddTransient<IBaseValidator<RegisterDto>, CustomRegisterDtoValidator>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

//Add  authorization policies

builder.Services.AddAuthorizationPolicies();
// Add additional custom policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole(RoleConstants.AdminRole, RoleConstants.UserRole));
    options.AddPolicy("RequireAdminRole", policy =>
       policy.RequireRole(RoleConstants.AdminRole));
});


// Add other services and configure middleware

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


## Usage

### Register a User

To register a new user, use the following endpoint:

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    var result = await _authService.RegisterAsync(registerDto);
    return !result.Succeeded ? BadRequest(result.Errors) : Ok(result);
}


[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    var result = await _authService.LoginAsync(loginDto);
    return !result.Succeeded ? Unauthorized(result.Errors) : Ok(result);
}

[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
{
    var result = await _authService.ResetPasswordAsync(resetPasswordDto);
    return !result.Succeeded ? BadRequest(result.Errors) : Ok(result);
}

````

## Extending

You can extend and customize the library according to your needs:

- **Roles and Policies**: Customize `RoleConstants` and `AuthorizationPolicies`.
- **Validators**: Modify or extend validators located in the `Validators` folder.
- **ApplicationUser**: Extend the `ApplicationUser` class to add additional properties.
- **Email Service**: Implement your own email logic by extending the `IEmailService` interface.

## Contributing

We welcome contributions! To contribute:

1. **Fork** the repository.
2. **Create** a new branch (`git checkout -b feature/your-feature`).
3. **Make** your changes.
4. **Commit** your changes (`git commit -am 'Add new feature'`).
5. **Push** to the branch (`git push origin feature/your-feature`).
6. **Create** a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For support or questions, please contact [muhdinmussema@gmail.com](mailto:muhdinmussema@gmail.com).
