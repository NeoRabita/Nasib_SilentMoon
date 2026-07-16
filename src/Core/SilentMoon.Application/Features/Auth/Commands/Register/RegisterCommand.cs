using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly IUow _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IAppLogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUow uow,
            IPasswordHasher passwordHasher,
            IOtpService otpService,
            IEmailService emailService,
            IAppLogger<RegisterCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> Handle(RegisterCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Register started for {Email}", command.Email);

            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            if (await _uow.UserRepository.ExistsByEmailAsync(normalizedEmail, ct))
                return AuthErrors.EmailNotUnique;

            var user = new ApplicationUser
            {
                FirstName = command.FirstName.Trim(),
                LastName = command.LastName.Trim(),
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.Hash(command.Password),
                IsEmailVerified = false
            };

            await _uow.UserRepository.AddAsync(user, ct);

            var otp = await _otpService.GenerateAsync(normalizedEmail);

            await _emailService.SendAsync(new EmailRequest
            {
                To = normalizedEmail,
                Subject = "SilentMoon - Email Verification",
                Body = $"<h3>Welcome, {user.FirstName}!</h3><p>Your verification code: <b>{otp}</b></p><p>This code expires in 5 minutes.</p>"
            });

            _logger.LogInformation("Register completed, OTP sent to {Email}", normalizedEmail);
            return Result.Success();
        }
    }
}
