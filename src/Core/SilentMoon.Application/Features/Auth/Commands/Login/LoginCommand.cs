using Application.Abstractions.Messaging;
using FluentValidation;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : ICommand<AuthenticationResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; } 
    }

    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthenticationResponse>
    {
        private readonly IUow _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IAppLogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUow uow,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IAppLogger<LoginCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(command.IpAddress);
            refreshToken.UserId = user.Id;

            await _uow.RefreshTokenRepository.AddAsync(refreshToken, ct);
            user.LastLoginAt = DateTime.UtcNow;
            _uow.UserRepository.Update(user);

            _logger.LogInformation("Login successful for user {UserId}", user.Id);

            return new AuthenticationResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = new RefreshTokenDto(refreshToken.Token, refreshToken.ExpiresAt)
            };
        }
    }

   
}
