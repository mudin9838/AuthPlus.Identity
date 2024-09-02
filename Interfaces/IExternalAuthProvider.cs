using AuthPlus.Identity.Dtos;

namespace AuthPlus.Identity.Interfaces;
public interface IExternalAuthProvider
{
    Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string token);

}
