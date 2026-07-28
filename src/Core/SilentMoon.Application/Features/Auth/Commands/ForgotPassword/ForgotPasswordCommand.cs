using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Auth.Events;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Enums;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : ICommand
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpDispatcher _otpDispatcher;
        private readonly IAppLogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IUow uow,
            IOtpDispatcher otpDispatcher,
            IAppLogger<ForgotPasswordCommandHandler> logger)
        {
            _uow = uow;
            _otpDispatcher = otpDispatcher;
            _logger = logger;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            if (user.AuthenticationProvider != AuthenticationProvider.Local)
                return AuthErrors.PasswordResetNotAllowedForExternalProvider;

            await _otpDispatcher.SendAsync(normalizedEmail, user.FirstName, OtpPurpose.PasswordReset);

            _logger.LogInformation("Password reset OTP dispatched for {Email}", normalizedEmail);
            return Result.Success();
        }
    }
}