using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using Newtonsoft.Json.Linq;

namespace AuthPlus.Identity.Services;
public class LinkedInAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;

    public LinkedInAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string token)
    {
        if (provider != "LinkedIn")
        {
            return null; // Handle unknown providers if needed
        }

        // Call LinkedIn's API to get user info
        var response = await _httpClient.GetAsync($"https://api.linkedin.com/v2/me?oauth2_access_token={token}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JObject.Parse(content);

        // Get additional user details
        var emailResponse = await _httpClient.GetAsync($"https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))&oauth2_access_token={token}");
        emailResponse.EnsureSuccessStatusCode();
        var emailContent = await emailResponse.Content.ReadAsStringAsync();
        var emailInfo = JObject.Parse(emailContent);

        var email = emailInfo["elements"]?[0]?["handle~"]?["emailAddress"]?.ToString();
        var name = userInfo["localizedFirstName"]?.ToString() + " " + userInfo["localizedLastName"]?.ToString();
        var profilePictureUrl = userInfo["profilePicture"]?["displayImage~"]?["elements"]?[0]?["identifiers"]?[0]?["identifier"]?.ToString();

        return new ExternalUserInfo
        {
            Email = email,
            Name = name,
            ProfilePictureUrl = profilePictureUrl
        };
    }
}