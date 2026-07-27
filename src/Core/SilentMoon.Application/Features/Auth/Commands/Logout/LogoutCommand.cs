using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : ICommand
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
    {
        private readonly IUow _uow;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAppLogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IUow uow,
            IDateTimeService dateTimeService,
            IAppLogger<LogoutCommandHandler> logger)
        {
            _uow = uow;
            _dateTimeService = dateTimeService;
            _logger = logger;
        }

        public async Task<Result> Handle(LogoutCommand command, CancellationToken ct)
        {
            var storedToken = await _uow.RefreshTokenRepository.GetByTokenAsync(command.RefreshToken, ct);

            if (storedToken is null || !storedToken.IsActive)
                return AuthErrors.InvalidRefreshToken;

            storedToken.RevokedAt = _dateTimeService.NowUtc;
            _uow.RefreshTokenRepository.Update(storedToken);

            _logger.LogInformation("User {UserId} logged out, token revoked", storedToken.UserId);
            return Result.Success();
        }
    }
}