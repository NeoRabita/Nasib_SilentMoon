using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : ICommand
    {
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; }
    }

    public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(IUow uow, IAppLogger<LogoutCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result> Handle(LogoutCommand command, CancellationToken ct)
        {
            var storedToken = await _uow.RefreshTokenRepository.GetByTokenAsync(command.RefreshToken, ct);

            if (storedToken is null || !storedToken.IsActive)
                return AuthErrors.InvalidRefreshToken;

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = command.IpAddress;
            _uow.RefreshTokenRepository.Update(storedToken);

            _logger.LogInformation("User {UserId} logged out, token revoked", storedToken.UserId);
            return Result.Success();
        }
    }
}
