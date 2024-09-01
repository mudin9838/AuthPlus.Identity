namespace AuthPlus.Identity.Entities;
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
}
