using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;   
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.DeleteReminder
{
    public class DeleteReminderCommand : ICommand
    {
        public int Id { get; set; }

        public DeleteReminderCommand() { }                
        public DeleteReminderCommand(int id) => Id = id;
    }

    public class DeleteReminderCommandHandler : ICommandHandler<DeleteReminderCommand>
    {
        private readonly IUow _uow;
        private readonly IUserService _userService;
        private readonly IAppLogger<DeleteReminderCommandHandler> _logger;

        public DeleteReminderCommandHandler(IUow uow, IUserService userService, IAppLogger<DeleteReminderCommandHandler> logger)
        {
            _uow = uow;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteReminderCommand command, CancellationToken ct)
        {
            var userId = _userService.GetUserIdAsInt();
            if (userId is null)
                return UserErrors.Unauthorized();

            var reminder = await _uow.ReminderRepository.GetByIdAsync(command.Id, ct);
            if (reminder is null)
                return ReminderErrors.NotFound(command.Id);

            if (reminder.UserId != userId.Value)
                return ReminderErrors.NotOwned;

            _uow.ReminderRepository.Delete(reminder);

            _logger.LogInformation("Reminder {ReminderId} deleted by user {UserId}", command.Id, userId);
            return Result.Success();
        }
    }
}