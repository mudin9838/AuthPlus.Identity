using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthPlus.Identity.Helpers;

public class JwtHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtHelper(string secretKey, string issuer, string audience)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(string userId, string userName, string[] roles)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userName),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

        // Add each role as a separate claim
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
