using System;

namespace SilentMoon.Application.DTOs.JWT
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }

        public RefreshTokenDto(string token, DateTimeOffset expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }
    }
}