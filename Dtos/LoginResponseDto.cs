namespace AuthPlus.Identity.Dtos;
public class LoginResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string UserName { get; set; }
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public IList<string> Roles { get; set; }
}