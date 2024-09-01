namespace AuthPlus.Identity.Dtos;
public class RegisterDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string[]? Roles { get; set; }
}