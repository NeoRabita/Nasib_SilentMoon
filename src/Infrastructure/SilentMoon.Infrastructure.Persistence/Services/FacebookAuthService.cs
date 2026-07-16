using System;
using System.Net.Http;
using System.Net.Http.Json;                      
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly FacebookAuthSettings _settings;
        private readonly IAppLogger<FacebookAuthService> _logger;

        public FacebookAuthService(
            HttpClient httpClient,
            IOptions<APIAppSettings> apiSettings,
            IAppLogger<FacebookAuthService> logger)
        {
            _httpClient = httpClient;
            _settings = apiSettings.Value.FacebookAuth;
            _logger = logger;
        }

        public async Task<ExternalUserInfo> ValidateAccessTokenAsync(string accessToken)
        {
            try
            {
                var appAccessToken = $"{_settings.AppId}|{_settings.AppSecret}";
                var debugResponse = await _httpClient.GetAsync(
                    $"https://graph.facebook.com/debug_token?input_token={Uri.EscapeDataString(accessToken)}&access_token={Uri.EscapeDataString(appAccessToken)}");

                if (!debugResponse.IsSuccessStatusCode)
                    return null;

                var debugResult = await debugResponse.Content.ReadFromJsonAsync<FacebookDebugTokenResponse>();
                var tokenData = debugResult?.Data;

                if (tokenData == null || !tokenData.IsValid ||
                    !string.Equals(tokenData.AppId, _settings.AppId, StringComparison.Ordinal))
                {
                    _logger.LogWarning("Facebook token is invalid or belongs to another app");
                    return null;
                }

                var meResponse = await _httpClient.GetAsync(
                    $"https://graph.facebook.com/me?fields=email,first_name,last_name&access_token={Uri.EscapeDataString(accessToken)}");

                if (!meResponse.IsSuccessStatusCode)
                    return null;

                var me = await meResponse.Content.ReadFromJsonAsync<FacebookMeResponse>();
                if (me == null)
                    return null;

                return new ExternalUserInfo
                {
                    Email = me.Email,
                    FirstName = me.FirstName,
                    LastName = me.LastName,
                    EmailVerified = !string.IsNullOrWhiteSpace(me.Email) 
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Facebook token validation failed");
                return null;
            }
        }

        private class FacebookDebugTokenResponse
        {
            [JsonPropertyName("data")] public FacebookTokenData Data { get; set; }
        }

        private class FacebookTokenData
        {
            [JsonPropertyName("app_id")] public string AppId { get; set; }
            [JsonPropertyName("is_valid")] public bool IsValid { get; set; }
        }

        private class FacebookMeResponse
        {
            [JsonPropertyName("email")] public string Email { get; set; }
            [JsonPropertyName("first_name")] public string FirstName { get; set; }
            [JsonPropertyName("last_name")] public string LastName { get; set; }
        }
    }
}
