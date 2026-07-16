using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : ICommand<ProfileViewModel>
    {
        public string FirstName { get; set; }   
        public string LastName { get; set; }    
        public string AvatarUrl { get; set; }   
    }

    public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, ProfileViewModel>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;
        private readonly IAppLogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IUow uow, IUserService userService, IAppLogger<UpdateProfileCommandHandler> logger)
        {
            _uow = uow;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result<ProfileViewModel>> Handle(UpdateProfileCommand command, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var user = await _uow.UserRepository.GetByIdAsync(userId.Value, ct);
            if (user is null)
                return AuthErrors.UserNotFound;

            if (command.FirstName is not null) user.FirstName = command.FirstName.Trim();
            if (command.LastName is not null) user.LastName = command.LastName.Trim();
            if (command.AvatarUrl is not null) user.AvatarUrl = command.AvatarUrl.Trim();

            _uow.UserRepository.Update(user);

            _logger.LogInformation("Profile updated for user {UserId}", user.Id);
            return user.ToProfileViewModel();
        }
    }
}
