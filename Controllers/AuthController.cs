using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using AuthPlus.Identity.Validators;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlus.Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IBaseValidator<RegisterDto> _registerDtoValidator;
    private readonly IBaseValidator<LoginDto> _loginDtoValidator;
    public AuthController(IAuthService authService, IBaseValidator<RegisterDto> registerDtoValidator, IBaseValidator<LoginDto> loginDtoValidator)
    {
        _authService = authService;
        _registerDtoValidator = registerDtoValidator;
        _loginDtoValidator = loginDtoValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var validationResult = await _loginDtoValidator.ValidateAsync(loginDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _authService.LoginAsync(loginDto);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var validationResult = await _registerDtoValidator.ValidateAsync(registerDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _authService.RegisterAsync(registerDto);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto externalLoginDto)
    {
        var result = await _authService.ExternalLoginAsync(externalLoginDto.Provider, externalLoginDto.Token);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
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
