using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.Features.Auth.Common;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Enums;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommand : ICommand<AuthenticationResponse>
    {
        public string IdToken { get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; }
    }

    public class GoogleLoginCommandHandler : ICommandHandler<GoogleLoginCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ExternalLoginProcessor _externalLoginProcessor;
        private readonly IAppLogger<GoogleLoginCommandHandler> _logger;

        public GoogleLoginCommandHandler(
            IGoogleAuthService googleAuthService,
            ExternalLoginProcessor externalLoginProcessor,
            IAppLogger<GoogleLoginCommandHandler> logger)
        {
            _googleAuthService = googleAuthService;
            _externalLoginProcessor = externalLoginProcessor;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(GoogleLoginCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Google login attempt started");

            var externalUser = await _googleAuthService.ValidateIdTokenAsync(command.IdToken);

            if (externalUser is null)
                return AuthErrors.InvalidExternalToken;

            var result = await _externalLoginProcessor.ProcessAsync(
                externalUser, AuthenticationProvider.Google, command.IpAddress, ct);

            if (result.IsSuccess)
                _logger.LogInformation("Google login successful for {Email}", externalUser.Email);

            return result;
        }
    }
}
