using AuthPlus.Identity.Dtos;

namespace AuthPlus.Identity.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(ForgotPasswordDto forgotPasswordDto);
}
