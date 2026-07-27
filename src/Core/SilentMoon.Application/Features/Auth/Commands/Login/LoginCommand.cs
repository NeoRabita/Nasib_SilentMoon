using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : ICommand<AuthenticationResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthenticationResponse>
    {
        private readonly IUow _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthTokenIssuer _tokenIssuer;
        private readonly IAppLogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUow uow,
            IPasswordHasher passwordHasher,
            IAuthTokenIssuer tokenIssuer,
            IAppLogger<LoginCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _tokenIssuer = tokenIssuer;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(LoginCommand command, CancellationToken ct)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();
            _logger.LogInformation("Login attempt for {Email}", normalizedEmail);

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);

            if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return AuthErrors.InvalidCredentials;

            if (!user.IsActive)
                return AuthErrors.UserInactive;

            if (!user.IsEmailVerified)
                return AuthErrors.EmailNotVerified;

            var response = await _tokenIssuer.IssueAsync(user, ct);

            _logger.LogInformation("Login successful for user {UserId}", user.Id);

            return response;
        }
    }
}