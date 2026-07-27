using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Profile.Queries.GetMe
{
    public class GetMeQuery : IQuery<ProfileViewModel>
    {
    }

    public class GetMeQueryHandler : IQueryHandler<GetMeQuery, ProfileViewModel>
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetMeQueryHandler(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<ProfileViewModel>> Handle(GetMeQuery query, CancellationToken ct)
        {
            var userResult = await _currentUserProvider.GetCurrentUserAsync(ct);
            if (userResult.IsFailure)
                return userResult.Error;

            return userResult.Value.ToProfileViewModel();
        }
    }
}