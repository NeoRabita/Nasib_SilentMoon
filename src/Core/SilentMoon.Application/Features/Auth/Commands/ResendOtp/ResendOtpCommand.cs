using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Auth.Events;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommand : ICommand
    {
        public string Email { get; set; }
    }

    public class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IOtpDispatcher _otpDispatcher;
        private readonly IAppLogger<ResendOtpCommandHandler> _logger;

        public ResendOtpCommandHandler(
            IUow uow,
            IOtpService otpService,
            IOtpDispatcher otpDispatcher,
            IAppLogger<ResendOtpCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _otpDispatcher = otpDispatcher;
            _logger = logger;
        }

        public async Task<Result> Handle(ResendOtpCommand command, CancellationToken ct)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            if (user.IsEmailVerified)
                return AuthErrors.EmailAlreadyVerified;

            await _otpService.RemoveAsync(normalizedEmail);
            await _otpDispatcher.SendAsync(normalizedEmail, user.FirstName, OtpPurpose.Resend);

            _logger.LogInformation("OTP resend dispatched for {Email}", normalizedEmail);
            return Result.Success();
        }
    }
}