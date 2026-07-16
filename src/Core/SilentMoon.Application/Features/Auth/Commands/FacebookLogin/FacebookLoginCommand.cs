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

namespace SilentMoon.Application.Features.Auth.Commands.FacebookLogin
{
    public class FacebookLoginCommand : ICommand<AuthenticationResponse>
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; }
    }

    public class FacebookLoginCommandHandler : ICommandHandler<FacebookLoginCommand, AuthenticationResponse>
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly ExternalLoginProcessor _externalLoginProcessor;
        private readonly IAppLogger<FacebookLoginCommandHandler> _logger;

        public FacebookLoginCommandHandler(
            IFacebookAuthService facebookAuthService,
            ExternalLoginProcessor externalLoginProcessor,
            IAppLogger<FacebookLoginCommandHandler> logger)
        {
            _facebookAuthService = facebookAuthService;
            _externalLoginProcessor = externalLoginProcessor;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(FacebookLoginCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Facebook login attempt started");

            var externalUser = await _facebookAuthService.ValidateAccessTokenAsync(command.AccessToken);

            if (externalUser is null)
                return AuthErrors.InvalidExternalToken;

         
            if (string.IsNullOrWhiteSpace(externalUser.Email))
                return AuthErrors.InvalidExternalToken;

            var result = await _externalLoginProcessor.ProcessAsync(
                externalUser, AuthenticationProvider.Facebook, command.IpAddress, ct);

            if (result.IsSuccess)
                _logger.LogInformation("Facebook login successful for {Email}", externalUser.Email);

            return result;
        }
    }
}
