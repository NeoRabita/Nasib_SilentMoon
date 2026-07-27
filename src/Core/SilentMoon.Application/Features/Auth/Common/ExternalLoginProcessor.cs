using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Common
{
    public class ExternalLoginProcessor
    {
        private readonly IUow _uow;
        private readonly IAuthTokenIssuer _tokenIssuer;

        public ExternalLoginProcessor(IUow uow, IAuthTokenIssuer tokenIssuer)
        {
            _uow = uow;
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result<AuthenticationResponse>> ProcessAsync(
            ExternalUserInfo externalUser,
            AuthenticationProvider provider,
            CancellationToken ct)
        {
            var normalizedEmail = externalUser.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(normalizedEmail, ct);

            if (user is null)
            {
                user = await CreateUserAsync(externalUser, normalizedEmail, provider, ct);
            }
            else
            {
                if (!user.IsActive)
                    return AuthErrors.UserInactive;

                if (user.AuthenticationProvider != provider)
                    return AuthErrors.WrongProvider;
            }

            return await _tokenIssuer.IssueAsync(user, ct);
        }

        private async Task<ApplicationUser> CreateUserAsync(
            ExternalUserInfo externalUser,
            string normalizedEmail,
            AuthenticationProvider provider,
            CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                FirstName = externalUser.FirstName ?? string.Empty,
                LastName = externalUser.LastName ?? string.Empty,
                Email = normalizedEmail,
                PasswordHash = null,
                IsEmailVerified = externalUser.EmailVerified,
                AuthenticationProvider = provider
            };

            await _uow.UserRepository.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return user;
        }
    }
}