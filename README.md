# AuthPlus.Identity

`AuthPlus.Identity` is a **.NET 8+ class library** designed to simplify **user authentication and authorization** in your ASP.NET Core applications. It provides ready-to-use APIs for users, roles, JWT authentication, email confirmation, password reset, and external login providers.

---

## Features

- User Registration and Login
- Password Reset and Email Confirmation
- User Management (CRUD)
- Role Management (CRUD, assign/remove roles)
- JWT-Based Authentication
- Configurable Email Service
- External Authentication Providers (Google, Facebook, LinkedIn, etc.)
- Extendable Validators and Policies
- Ready-to-use Controllers and Endpoints
- Supports SQL Server, PostgreSQL (via EF Core)

---

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- NuGet Package Manager

### Installation

Add the package to your project:

```bash
dotnet add package AuthPlus.Identity
```

---

## Configuration

Add the following settings to your `appsettings.json`:

```json
{
  "JwtSettings": {
   "SecretKey": "your-32-character-super-secret-key-here", // Min 32 chars
    "Issuer": "your-app-name",
    "Audience": "your-app-clients",
    "ExpirationMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@example.com",
    "SmtpPassword": "your-email-password",
    "BaseUrl": "https://your-app-url"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=AuthDb;Trusted_Connection=True;"
  }
}
```

> **Optional**: Add external login providers by implementing `IExternalAuthProvider` (e.g., Google, Facebook, LinkedIn).

---

## Setup in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add AuthPlus Identity (select your database provider)
builder.Services.AddAuthPlusIdentity(builder.Configuration, options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    // options.UsePostgres(...);
    // options.UseMySql(...);
});

// Register external authentication providers (optional)
builder.Services.AddHttpClient<GoogleAuthProvider>();
builder.Services.AddHttpClient<MicrosoftAuthProvider>();
builder.Services.AddScoped<IExternalAuthProvider, GoogleAuthProvider>();
builder.Services.AddScoped<IExternalAuthProvider, MicrosoftAuthProvider>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();
// IMPORTANT: Middleware must be in correct order
app.UseRouting(); // This must come first!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

> `AddAuthPlusIdentity` automatically registers:
> - Identity services
> - JWT authentication
> - Email service
> - IUserService, IRoleService, IAuthService
> - Policies (`RequireAdminRole`, `RequireUserRole`, `RequireAdminOrUserRole`)
> - Validators for DTOs (`LoginDto`, `RegisterDto`, `UserDto`, `ResetPasswordDto`)

---

## Controllers & Endpoints

### AuthController

| Endpoint                    | Method | Description |
|------------------------------|--------|-------------|
| `/api/auth/register`         | POST   | Register a new user |
| `/api/auth/login`            | POST   | Login a user |
| `/api/auth/external-login`   | POST   | Login via external provider (Google, Facebook, etc.) |
| `/api/auth/refresh-token`    | POST   | Refresh JWT token |
| `/api/auth/forgot-password`  | POST   | Send password reset email |
| `/api/auth/reset-password`   | POST   | Reset user password |

### UserController

| Endpoint                                     | Method | Authorization | Description |
|----------------------------------------------|--------|---------------|-------------|
| `/api/user`                                  | GET    | Admin          | Get all users |
| `/api/user/{id}`                             | GET    | User/Admin     | Get user by ID |
| `/api/user`                                  | POST   | Admin          | Create a user |
| `/api/user/{id}`                             | PUT    | User/Admin     | Update user info |
| `/api/user/{id}`                             | DELETE | Admin          | Delete a user |
| `/api/user/{userId}/roles/{roleName}`       | POST   | Admin          | Assign role to user |
| `/api/user/{userId}/roles/{roleName}`       | DELETE | Admin          | Remove role from user |
| `/api/user/confirm-email?token=&userId=`   | GET    | Anonymous      | Confirm user email |

### RoleController

| Endpoint           | Method | Authorization | Description |
|-------------------|--------|---------------|-------------|
| `/api/role/all`    | GET    | Admin          | Get all roles |
| `/api/role/{id}`   | GET    | Admin          | Get role by ID |
| `/api/role`        | POST   | Admin          | Create a new role |
| `/api/role/{id}`   | PUT    | Admin          | Update role |
| `/api/role/{id}`   | DELETE | Admin          | Delete role |

---

## Extending

You can extend and customize the library:

- **Roles & Policies**: Extend `AuthorizationPolicies` or add custom roles.
- **Validators**: Override or create custom DTO validators.
- **ApplicationUser**: Add new properties to your user class.
- **Email Service**: Implement `IEmailService` for custom email logic.
- **External Providers**: Implement `IExternalAuthProvider` for social logins.

---

## Optional: Override Default Validators

```csharp
public class CustomRegisterDtoValidator : RegisterDtoValidator
{
    public CustomRegisterDtoValidator()
    {
        RuleFor(x => x.Password)
            .Must(p => p.Contains("@"))
            .WithMessage("Password must contain '@'.");

        RuleFor(x => x.Password)
            .Matches(@"\d")
            .WithMessage("Password must contain at least one number.");
    }
}

// Register in Program.cs to replace default validator
builder.Services.AddTransient<IBaseValidator<RegisterDto>, CustomRegisterDtoValidator>();
```

---

## Example: Register User in Controller

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    var result = await _authService.RegisterAsync(registerDto);
    return !result.Succeeded ? BadRequest(result.Errors) : Ok(result);
}
```

---

## License & Support

- MIT License  
- For questions or support: [muhdinmussema@gmail.com](mailto:muhdinmussema@gmail.com)

