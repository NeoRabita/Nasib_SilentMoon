using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
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
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAppLogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(
            IUow uow,
            ICurrentUserProvider currentUserProvider,
            IAppLogger<UpdateProfileCommandHandler> logger)
        {
            _uow = uow;
            _currentUserProvider = currentUserProvider;
            _logger = logger;
        }

        public async Task<Result<ProfileViewModel>> Handle(UpdateProfileCommand command, CancellationToken ct)
        {
            var userResult = await _currentUserProvider.GetCurrentUserAsync(ct);
            if (userResult.IsFailure)
                return userResult.Error;

            var user = userResult.Value;

            if (command.FirstName is not null) user.FirstName = command.FirstName.Trim();
            if (command.LastName is not null) user.LastName = command.LastName.Trim();
            if (command.AvatarUrl is not null) user.AvatarUrl = command.AvatarUrl.Trim();

            _uow.UserRepository.Update(user);

            _logger.LogInformation("Profile updated for user {UserId}", user.Id);
            return user.ToProfileViewModel();
        }
    }
}