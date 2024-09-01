namespace AuthPlus.Identity.Dtos;
public class ForgotPasswordDto
{
    public string Email { get; set; }
    public string ResetLink { get; set; }
}