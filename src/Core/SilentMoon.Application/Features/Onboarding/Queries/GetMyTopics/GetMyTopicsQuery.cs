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

namespace SilentMoon.Application.Features.Onboarding.Queries.GetMyTopics
{
    public class GetMyTopicsQuery : IQuery<List<TopicViewModel>>
    {
    }

    public class GetMyTopicsQueryHandler : IQueryHandler<GetMyTopicsQuery, List<TopicViewModel>>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;

        public GetMyTopicsQueryHandler(IUow uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task<Result<List<TopicViewModel>>> Handle(GetMyTopicsQuery query, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var topics = await _uow.TopicRepository.GetUserTopicsAsync(userId.Value, ct);
            return topics.Select(t => t.ToTopicViewModel()).ToList();
        }
    }
}
