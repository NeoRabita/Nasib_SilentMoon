using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Features.Onboarding.Events;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.CreateReminder
{
    public class CreateReminderCommand : ICommand<ReminderViewModel>
    {
        public string Time { get; set; }   
        public int Days { get; set; }      
        public bool IsActive { get; set; } = true;
    }

    public class CreateReminderCommandHandler : ICommandHandler<CreateReminderCommand, ReminderViewModel>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;
        private readonly IEventPublisher _publisher;
        private readonly IAppLogger<CreateReminderCommandHandler> _logger;

        public CreateReminderCommandHandler(IUow uow, IUserService userService, IEventPublisher publisher, IAppLogger<CreateReminderCommandHandler> logger)
        {
            _uow = uow;
            _userService = userService;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<Result<ReminderViewModel>> Handle(CreateReminderCommand command, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var reminder = new Reminder
            {
                UserId = userId.Value,
                Time = command.Time,
                Days = command.Days,
                IsActive = command.IsActive
            };

            await _uow.ReminderRepository.AddAsync(reminder, ct);
            await _uow.SaveChangesAsync(ct);
            await _publisher.PublishAsync("reminder.created", new ReminderChangedEvent
            {
                ReminderId = reminder.Id,
                UserId = reminder.UserId,
                Time = reminder.Time,
                Days = reminder.Days,
                IsActive = reminder.IsActive
            });

            _logger.LogInformation("Reminder {ReminderId} created for user {UserId}", reminder.Id, userId);
            return reminder.ToReminderViewModel();
        }
    }
}
