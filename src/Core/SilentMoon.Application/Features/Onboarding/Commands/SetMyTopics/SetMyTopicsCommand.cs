using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.SetMyTopics
{
    public class SetMyTopicsCommand : ICommand
    {
        public List<int> TopicIds { get; set; } = new();
    }

    public class SetMyTopicsCommandHandler : ICommandHandler<SetMyTopicsCommand>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;
        private readonly IAppLogger<SetMyTopicsCommandHandler> _logger;

        public SetMyTopicsCommandHandler(IUow uow, IUserService userService, IAppLogger<SetMyTopicsCommandHandler> logger)
        {
            _uow = uow;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result> Handle(SetMyTopicsCommand command, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var distinctIds = command.TopicIds.Distinct().ToList();

            if (distinctIds.Count > 0)
            {
                var existingCount = await _uow.TopicRepository.CountExistingActiveAsync(distinctIds, ct);
                if (existingCount != distinctIds.Count)
                    return TopicErrors.SomeNotFound;
            }

            await _uow.TopicRepository.ReplaceUserTopicsAsync(userId.Value, distinctIds, ct);

            _logger.LogInformation("User {UserId} topics replaced with [{Topics}]", userId, string.Join(",", distinctIds));
            return Result.Success();
        }
    }
}
