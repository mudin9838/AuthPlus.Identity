# AuthPlus.Identity

AuthPlus.Identity is a .NET 8 class library designed to simplify user authentication and authorization management.

## Features

- User Registration and Login
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
    "SmtpPassword": "your-email-password"
  }
}


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
    "SmtpPassword": "your-email-password"
  }
}

# Setting Up
### In your Program.cs, configure the services and middleware:

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

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
