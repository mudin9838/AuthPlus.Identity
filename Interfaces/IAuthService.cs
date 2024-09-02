using AuthPlus.Identity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthPlus.Identity.Interfaces;
public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto);
    Task<AuthenticationResult> LoginAsync(LoginDto loginDto);
    Task<AuthenticationResult> ExternalLoginAsync(string provider, string token);
    Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<AuthenticationResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task SendResetPasswordEmailAsync(ForgotPasswordDto forgotPasswordDto);
    Task<IdentityResult> ConfirmEmailAsync(string token, string userId);
}