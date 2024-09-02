namespace AuthPlus.Identity.Dtos;
public class ExternalLoginDto
{
    public string Provider { get; set; } // e.g., "Google" or "Microsoft"
    public string Token { get; set; }
}