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
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.UpdateReminder
{
    public class UpdateReminderCommand : ICommand<ReminderViewModel>
    {
        [JsonIgnore]
        public int Id { get; set; }        

        public string Time { get; set; }   
        public int? Days { get; set; }     
        public bool? IsActive { get; set; } 
    }

    public class UpdateReminderCommandHandler : ICommandHandler<UpdateReminderCommand, ReminderViewModel>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;
        private readonly IAppLogger<UpdateReminderCommandHandler> _logger;

        public UpdateReminderCommandHandler(IUow uow, IUserService userService, IAppLogger<UpdateReminderCommandHandler> logger)
        {
            _uow = uow;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result<ReminderViewModel>> Handle(UpdateReminderCommand command, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var reminder = await _uow.ReminderRepository.GetByIdAsync(command.Id, ct);
            if (reminder is null)
                return ReminderErrors.NotFound(command.Id);

            if (reminder.UserId != userId.Value)
                return ReminderErrors.NotOwned;

            if (command.Time is not null) reminder.Time = command.Time;
            if (command.Days.HasValue) reminder.Days = command.Days.Value;
            if (command.IsActive.HasValue) reminder.IsActive = command.IsActive.Value;

            _uow.ReminderRepository.Update(reminder);

            _logger.LogInformation("Reminder {ReminderId} updated by user {UserId}", reminder.Id, userId);
            return reminder.ToReminderViewModel();
        }
    }
}
