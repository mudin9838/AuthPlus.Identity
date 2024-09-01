namespace AuthPlus.Identity.Dtos;

public class AuthenticationResult
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string UserName { get; set; }
    public string[] Roles { get; set; }
    public string ProfileImageUrl { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
