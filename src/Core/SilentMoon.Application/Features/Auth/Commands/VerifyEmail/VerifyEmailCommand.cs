using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommand : ICommand
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IAppLogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(IUow uow, IOtpService otpService, IAppLogger<VerifyEmailCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _logger = logger;
        }

        public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken ct)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            if (user.IsEmailVerified)
                return AuthErrors.EmailAlreadyVerified;

            var isValid = await _otpService.VerifyAsync(normalizedEmail, command.OtpCode);
            if (!isValid)
                return AuthErrors.InvalidOtp;

            user.IsEmailVerified = true;
            _uow.UserRepository.Update(user);

            await _otpService.RemoveAsync(normalizedEmail);

            _logger.LogInformation("Email verified for user {UserId}", user.Id);
            return Result.Success();
        }
    }
}
