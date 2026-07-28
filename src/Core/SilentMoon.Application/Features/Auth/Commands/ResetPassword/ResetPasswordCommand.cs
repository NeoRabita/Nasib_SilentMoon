using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Enums;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : ICommand
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppLogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IUow uow,
            IOtpService otpService,
            IPasswordHasher passwordHasher,
            IAppLogger<ResetPasswordCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken ct)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            if (user.AuthenticationProvider != AuthenticationProvider.Local)
                return AuthErrors.PasswordResetNotAllowedForExternalProvider;

            var isValid = await _otpService.VerifyAsync(normalizedEmail, command.OtpCode);
            if (!isValid)
                return AuthErrors.InvalidOtp;

            user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
            _uow.UserRepository.Update(user);

            await _otpService.RemoveAsync(normalizedEmail);

            _logger.LogInformation("Password reset completed for user {UserId}", user.Id);
            return Result.Success();
        }
    }
}