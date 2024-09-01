using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using System.Net.Mail;
using System.Net;

namespace AuthPlus.Identity.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
    {
        _smtpClient = new SmtpClient(smtpServer)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true,
        };

        _senderEmail = senderEmail;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendPasswordResetEmailAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var subject = "Password Reset Request";
        var body = $"Please reset your password by clicking the following link: <a href=\"{forgotPasswordDto.ResetLink}\">Reset Password</a>";

        await SendEmailAsync(forgotPasswordDto.Email, subject, body);
    }
}
