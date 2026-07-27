using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : ICommand<AuthenticationResponse>
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
    {
        private readonly IUow _uow;
        private readonly IJwtService _jwtService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAppLogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IUow uow,
            IJwtService jwtService,
            IDateTimeService dateTimeService,
            IAppLogger<RefreshTokenCommandHandler> logger)
        {
            _uow = uow;
            _jwtService = jwtService;
            _dateTimeService = dateTimeService;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand command, CancellationToken ct)
        {
            var storedToken = await _uow.RefreshTokenRepository.GetByTokenAsync(command.RefreshToken, ct);

            if (storedToken is null || !storedToken.IsActive)
                return AuthErrors.InvalidRefreshToken;

            var user = storedToken.User;

            if (user is null || !user.IsActive)
                return AuthErrors.UserInactive;

            storedToken.RevokedAt = _dateTimeService.NowUtc;
            _uow.RefreshTokenRepository.Update(storedToken);

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;
            await _uow.RefreshTokenRepository.AddAsync(newRefreshToken, ct);

            var accessToken = _jwtService.GenerateAccessToken(user);

            _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

            return new AuthenticationResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = new RefreshTokenDto(newRefreshToken.Token, newRefreshToken.ExpiresAt)
            };
        }
    }
}