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

namespace SilentMoon.Application.Features.Onboarding.Queries.GetMyReminders
{
    public class GetMyRemindersQuery : IQuery<List<ReminderViewModel>>
    {
    }

    public class GetMyRemindersQueryHandler : IQueryHandler<GetMyRemindersQuery, List<ReminderViewModel>>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;

        public GetMyRemindersQueryHandler(IUow uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task<Result<List<ReminderViewModel>>> Handle(GetMyRemindersQuery query, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var reminders = await _uow.ReminderRepository.GetUserRemindersAsync(userId.Value, ct);
            return reminders.Select(r => r.ToReminderViewModel()).ToList();
        }
    }
}
