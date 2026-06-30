using System;

namespace SilentMoon.Application.DTOs.JWT
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }

        public JwtTokenDto(string token, DateTimeOffset expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }
    }
}