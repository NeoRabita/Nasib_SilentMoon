using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;      
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;       
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Infrastructure.Persistence.Settings;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class JwtService : IJwtService
    {
        private readonly JWTSettings _jwtSettings;

        public JwtService(IOptions<APIAppSettings> apiSettings)
        {
            _jwtSettings = apiSettings.Value.JWTSettings;
        }

        public JwtTokenDto GenerateAccessToken(ApplicationUser user)
        {
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtTokenDto(tokenString, expiresAt);
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDuration)
            };
        }
    }
}
