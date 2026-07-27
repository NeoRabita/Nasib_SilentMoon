using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Common.Services
{
    public class AuthTokenIssuer : IAuthTokenIssuer
    {
        private readonly IUow _uow;
        private readonly IJwtService _jwtService;
        private readonly IDateTimeService _dateTimeService;

        public AuthTokenIssuer(IUow uow, IJwtService jwtService, IDateTimeService dateTimeService)
        {
            _uow = uow;
            _jwtService = jwtService;
            _dateTimeService = dateTimeService;
        }

        public async Task<AuthenticationResponse> IssueAsync(ApplicationUser user, CancellationToken ct)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            await _uow.RefreshTokenRepository.AddAsync(refreshToken, ct);

            user.LastLoginAt = _dateTimeService.NowUtc;
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