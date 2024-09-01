using System.Security.Claims;

namespace AuthPlus.Identity.Helpers;

public class RefreshTokenHelper
{
    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        // Logic to validate the refresh token and get the principal
        // This should match the implementation of JWT token validation
        // For simplicity, assume token is valid and return a dummy principal
        var identity = new ClaimsIdentity();
        return new ClaimsPrincipal(identity);
    }
}
