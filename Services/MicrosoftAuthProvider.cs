namespace AuthPlus.Identity.Services;
using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class MicrosoftAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;

    public MicrosoftAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string token)
    {
        if (provider != "Microsoft")
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/me?access_token={token}");
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        dynamic userInfo = JsonConvert.DeserializeObject(content);

        return new ExternalUserInfo
        {
            Email = userInfo.mail ?? userInfo.userPrincipalName,
            Name = userInfo.displayName,
            ProfilePictureUrl = userInfo.photo?.photoUrl // Adapt as necessary
        };
    }
}
