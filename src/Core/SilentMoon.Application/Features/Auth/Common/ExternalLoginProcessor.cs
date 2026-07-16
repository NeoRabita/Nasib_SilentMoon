using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Common
{
    public class ExternalLoginProcessor
    {
        private readonly IUow _uow;
        private readonly IJwtService _jwtService;

        public ExternalLoginProcessor(IUow uow, IJwtService jwtService)
        {
            _uow = uow;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthenticationResponse>> ProcessAsync(
            ExternalUserInfo externalUser,
            AuthenticationProvider provider,
            string ipAddress,
            CancellationToken ct)
        {
            var normalizedEmail = externalUser.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    FirstName = externalUser.FirstName ?? string.Empty,
                    LastName = externalUser.LastName ?? string.Empty,
                    Email = normalizedEmail,
                    PasswordHash = null, 
                    IsEmailVerified = externalUser.EmailVerified,
                    AuthenticationProvider = provider
                };

                await _uow.UserRepository.AddAsync(user, ct);
                await _uow.SaveChangesAsync(ct);
            }
            else
            {
                if (!user.IsActive)
                    return AuthErrors.UserInactive;

                if (user.AuthenticationProvider != provider)
                    return AuthErrors.WrongProvider;
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            await _uow.RefreshTokenRepository.AddAsync(refreshToken, ct);
            user.LastLoginAt = DateTime.UtcNow;
            _uow.UserRepository.Update(user);

            return new AuthenticationResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = new RefreshTokenDto(refreshToken.Token, refreshToken.ExpiresAt)
            };
        }
    }
    }
