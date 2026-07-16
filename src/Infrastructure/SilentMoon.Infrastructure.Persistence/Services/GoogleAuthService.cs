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
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleAuthSettings _settings;
        private readonly IAppLogger<GoogleAuthService> _logger;

        public GoogleAuthService(
            HttpClient httpClient,
            IOptions<APIAppSettings> apiSettings,
            IAppLogger<GoogleAuthService> logger)
        {
            _httpClient = httpClient;
            _settings = apiSettings.Value.GoogleAuth;
            _logger = logger;
        }

        public async Task<ExternalUserInfo> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={Uri.EscapeDataString(idToken)}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var payload = await response.Content.ReadFromJsonAsync<GoogleTokenPayload>();
                if (payload == null)
                    return null;

                if (!string.Equals(payload.Aud, _settings.ClientId, StringComparison.Ordinal))
                {
                    _logger.LogWarning("Google token audience mismatch");
                    return null;
                }

                if (long.TryParse(payload.Exp, out var exp) &&
                    DateTimeOffset.FromUnixTimeSeconds(exp) < DateTimeOffset.UtcNow)
                    return null;

                return new ExternalUserInfo
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailVerified = string.Equals(payload.EmailVerified, "true", StringComparison.OrdinalIgnoreCase)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Google token validation failed");
                return null;
            }
        }

        private class GoogleTokenPayload
        {
            [JsonPropertyName("aud")] public string Aud { get; set; }
            [JsonPropertyName("exp")] public string Exp { get; set; }
            [JsonPropertyName("email")] public string Email { get; set; }
            [JsonPropertyName("email_verified")] public string EmailVerified { get; set; }
            [JsonPropertyName("given_name")] public string GivenName { get; set; }
            [JsonPropertyName("family_name")] public string FamilyName { get; set; }
        }
    }
}
