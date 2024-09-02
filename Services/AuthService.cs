﻿using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Entities;
using AuthPlus.Identity.Interfaces;
using AuthPlus.Identity.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AuthPlus.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHelper _jwtHelper;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    public AuthService(UserManager<ApplicationUser> userManager, JwtHelper jwtHelper, IEmailService emailService, IOptions<EmailSettings> emailSettings)
    {
        _userManager = userManager;
        _jwtHelper = jwtHelper;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = ["Passwords do not match."]
            };
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            ProfileImageUrl = registerDto.ProfileImageUrl
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        var roles = registerDto.Roles ?? ["User"];
        await _userManager.AddToRolesAsync(user, roles);

        // Generate email confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_emailSettings.BaseUrl}/confirm-email?token={token}&userId={user.Id}";

        // Send confirmation email
        var subject = "Confirm Your Email";
        var body = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";
        await _emailService.SendEmailAsync(user.Email, subject, body);

        var jwtToken = _jwtHelper.GenerateToken(user.UserName, roles);
        var refreshToken = Guid.NewGuid().ToString(); // Simplified refresh token generation

        return new AuthenticationResult
        {
            Succeeded = true,
            Token = jwtToken,
            RefreshToken = refreshToken,
            UserName = user.UserName,
            Roles = roles,
            ProfileImageUrl = user.ProfileImageUrl
        };
    }
    public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid username or password." }
            };
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid username or password." }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateToken(user.UserName, roles.ToArray());
        var refreshToken = Guid.NewGuid().ToString(); // Simplified refresh token generation

        return new AuthenticationResult
        {
            Succeeded = true,
            Token = token,
            RefreshToken = refreshToken,
            UserName = user.UserName,
            Roles = roles.ToArray(),
            ProfileImageUrl = user.ProfileImageUrl
        };
    }
    public async Task<IdentityResult> ConfirmEmailAsync(string token, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null
            ? IdentityResult.Failed(new IdentityError { Description = "Invalid user ID." })
            : await _userManager.ConfirmEmailAsync(user, token);
    }
    public async Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        // Extract user name from the refresh token
        var user = await _userManager.FindByNameAsync(refreshTokenDto.UserName);
        if (user == null)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid user." }
            };
        }

        // Verify the refresh token (you would need to implement actual verification logic)
        if (refreshTokenDto.RefreshToken != "expected-refresh-token") // Placeholder check
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid refresh token." }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateToken(user.UserName, roles.ToArray());
        var newRefreshToken = Guid.NewGuid().ToString(); // Generate new refresh token

        return new AuthenticationResult
        {
            Succeeded = true,
            Token = token,
            RefreshToken = newRefreshToken,
            UserName = user.UserName,
            Roles = roles.ToArray(),
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public async Task<AuthenticationResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid email address." }
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        return !result.Succeeded
            ? new AuthenticationResult
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            }
            : new AuthenticationResult
            {
                Succeeded = true
            };
    }

    public async Task SendResetPasswordEmailAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            // Handle the case where the user is not found
            return; // Optionally log or handle this scenario
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{_emailSettings.BaseUrl}/reset-password?token={token}";

        forgotPasswordDto.ResetLink = resetLink;
        await _emailService.SendPasswordResetEmailAsync(forgotPasswordDto);
    }
}
