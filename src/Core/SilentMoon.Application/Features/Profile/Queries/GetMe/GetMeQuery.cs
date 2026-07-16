using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Profile.Queries.GetMe
{
    public class GetMeQuery : IQuery<ProfileViewModel>
    {
    }

    public class GetMeQueryHandler : IQueryHandler<GetMeQuery, ProfileViewModel>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;

        public GetMeQueryHandler(IUow uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task<Result<ProfileViewModel>> Handle(GetMeQuery query, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();

            if (userId is null)
                return UserErrors.Unauthorized();

            var user = await _uow.UserRepository.GetByIdAsync(userId.Value, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            return user.ToProfileViewModel();
        }
    }
}
