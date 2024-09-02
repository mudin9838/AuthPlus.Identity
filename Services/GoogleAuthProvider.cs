using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using Newtonsoft.Json;

namespace AuthPlus.Identity.Services;
public class GoogleAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;

    public GoogleAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string token)
    {
        if (provider != "Google")
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={token}");
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        dynamic userInfo = JsonConvert.DeserializeObject(content);

        return new ExternalUserInfo
        {
            Email = userInfo.email,
            Name = userInfo.name,
            ProfilePictureUrl = userInfo.picture
        };
    }
}