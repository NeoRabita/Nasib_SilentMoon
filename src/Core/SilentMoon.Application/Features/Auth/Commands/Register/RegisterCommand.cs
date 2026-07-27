using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Auth.Events;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Errors;
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
        private readonly IOtpDispatcher _otpDispatcher;
        private readonly IAppLogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUow uow,
            IPasswordHasher passwordHasher,
            IOtpDispatcher otpDispatcher,
            IAppLogger<RegisterCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _otpDispatcher = otpDispatcher;
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

            await _otpDispatcher.SendAsync(normalizedEmail, user.FirstName, OtpPurpose.Register);

            _logger.LogInformation("Register completed, OTP dispatched for {Email}", normalizedEmail);
            return Result.Success();
        }
    }
}