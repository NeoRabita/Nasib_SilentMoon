using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Mappings;
using SilentMoon.Domain.Errors;

namespace SilentMoon.Application.Features.Pomodoros.Commands.CreatePomodoro
{
    public partial class CreatePomodoroCommand : ICommand<string>
    {
        public string Name { get; set; }
        public int PomodoroTime { get; set; }
        public int ShortBreakTime { get; set; }
        public int LongBreakTime { get; set; }
        public int LongBreakInterval { get; set; }
        public int PeriodCount { get; set; }
        public int Color { get; set; }
    }

    public class CreatePomodoroCommandHandler : ICommandHandler<CreatePomodoroCommand, string>
    {
        private readonly IUserService userService;
        private readonly IUow _uow;
        private readonly IAppLogger<CreatePomodoroCommandHandler> _logger;

        public CreatePomodoroCommandHandler(IUserService userService, IUow uow, IAppLogger<CreatePomodoroCommandHandler> logger)
        {
            this.userService = userService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CreatePomodoroCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreatePomodoro started.");

            var userIdStr = userService.GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return UserErrors.Unauthorized();

            var pomodoro = command.ToPomodoro();
            pomodoro.UserId = userId;

            await _uow.PomodoroRepository.AddAsync(pomodoro);
            await _uow.SaveChangesAsync(cancellationToken);

            var result = await _uow.PomodoroRepository.CreatePomodoroLog(pomodoro.Id);

            _logger.LogInformation("Pomodoro successfully created. ID: {PomodoroId}", pomodoro.Id);
            return result;
        }
    }
}