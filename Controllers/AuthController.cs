using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlus.Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return result.Succeeded ? Ok(result) : Unauthorized(result.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        return result.Succeeded ? Ok("User registered successfully") : BadRequest(result.Errors);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenDto);
        return result.Succeeded ? Ok(result) : Unauthorized(result.Errors);
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        _authService.SendResetPasswordEmailAsync(forgotPasswordDto);
        return Ok("Reset password email sent");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);
        return result.Succeeded ? Ok("Password reset successfully") : BadRequest(result.Errors);
    }
}
