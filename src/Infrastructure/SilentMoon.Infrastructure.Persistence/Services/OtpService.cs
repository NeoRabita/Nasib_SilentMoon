using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class OtpService : IOtpService
    {
        private readonly ICacheService _cacheService;

        private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);
        private const int MaxAttempts = 5;

        public OtpService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        private static string OtpKey(string email) => $"otp:{email}";
        private static string AttemptsKey(string email) => $"otp-attempts:{email}";

        public async Task<string> GenerateAsync(string email)
        {
            var otp = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

            await _cacheService.SetAsync(OtpKey(email), otp, OtpLifetime);
            await _cacheService.SetAsync(AttemptsKey(email), 0, OtpLifetime);

            return otp;
        }

        public async Task<bool> VerifyAsync(string email, string otp)
        {
            var storedOtp = await _cacheService.GetAsync<string>(OtpKey(email));
            if (storedOtp == null)
                return false; 

            var attempts = await _cacheService.GetAsync<int?>(AttemptsKey(email)) ?? 0;
            if (attempts >= MaxAttempts)
            {
                await RemoveAsync(email);
                return false;
            }

            await _cacheService.SetAsync(AttemptsKey(email), attempts + 1, OtpLifetime);

            return string.Equals(storedOtp, otp, StringComparison.Ordinal);
        }

        public async Task RemoveAsync(string email)
        {
            await _cacheService.RemoveAsync(OtpKey(email));
            await _cacheService.RemoveAsync(AttemptsKey(email));
        }
    }
}
