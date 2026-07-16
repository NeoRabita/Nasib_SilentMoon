using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IEmailService _emailService;
        private readonly IAppLogger<ResendOtpCommandHandler> _logger;

        public ResendOtpCommandHandler(
            IUow uow,
            IOtpService otpService,
            IEmailService emailService,
            IAppLogger<ResendOtpCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _emailService = emailService;
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
            var otp = await _otpService.GenerateAsync(normalizedEmail);

            await _emailService.SendAsync(new EmailRequest
            {
                To = normalizedEmail,
                Subject = "SilentMoon - Email Verification (Resend)",
                Body = $"<h3>Hello, {user.FirstName}!</h3><p>Your new verification code: <b>{otp}</b></p><p>This code expires in 5 minutes.</p>"
            });

            _logger.LogInformation("OTP resent to {Email}", normalizedEmail);
            return Result.Success();
        }
    }
}
