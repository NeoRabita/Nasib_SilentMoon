using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Common.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;

        public CurrentUserProvider(IUow uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task<Result<ApplicationUser>> GetCurrentUserAsync(CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var user = await _uow.UserRepository.GetByIdAsync(userId.Value, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            return user;
        }
    }
}