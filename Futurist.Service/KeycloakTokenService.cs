using System.Text;
using System.Text.Json.Serialization;
using Futurist.Service.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Futurist.Service;

public class KeycloakTokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public KeycloakTokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var keycloakConfig = _configuration.GetSection("Auth");
        var tokenEndpoint = $"{keycloakConfig["Authority"]}/protocol/openid-connect/token";

        var clientId = keycloakConfig["ClientId"];
        var clientSecret = keycloakConfig["ClientSecret"];

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var requestContent = new FormUrlEncodedContent(requestBody);

        var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve access token: {response.ReasonPhrase}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse?.AccessToken ?? throw new Exception("Access token not found in response");
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    
    public async Task Notify(NotificationDto<IEnumerable<int>> notificationDto)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        // set room ids to the body
        var content = new StringContent(JsonConvert.SerializeObject(notificationDto), Encoding.UTF8, "application/json");
        var baseUrl = _configuration.GetSection("BaseUrl").Value;
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new Exception("BaseUrl is not set");
        }
        var url = $"{baseUrl}/api/Notification/Notify";
        await _httpClient.PostAsync(url, content);
    }
}