using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthPlus.Identity.Helpers;

public class JwtHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtHelper(string secretKey, string issuer, string audience)
    {
        _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));

        // Debug output
        Console.WriteLine($"[JwtHelper] Initialized with:");
        Console.WriteLine($"[JwtHelper]   Issuer: {_issuer}");
        Console.WriteLine($"[JwtHelper]   Audience: {_audience}");
        Console.WriteLine($"[JwtHelper]   SecretKey length: {_secretKey.Length}");
    }

    public string GenerateToken(string userId, string userName, string[] roles)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentNullException(nameof(userName));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add roles
        if (roles != null && roles.Length > 0)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Debug: Decode and verify token
        var decoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        Console.WriteLine($"[JwtHelper] Generated token details:");
        Console.WriteLine($"[JwtHelper]   Token Issuer: {decoded.Issuer}");
        Console.WriteLine($"[JwtHelper]   Token Audience: {string.Join(", ", decoded.Audiences)}");
        Console.WriteLine($"[JwtHelper]   Token Expires: {decoded.ValidTo}");
        Console.WriteLine($"[JwtHelper]   Token Claims: {decoded.Claims.Count()}");

        return tokenString;
    }

    // Helper method to validate token (for debugging)
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[JwtHelper] Token validation failed: {ex.Message}");
            return false;
        }
    }
}