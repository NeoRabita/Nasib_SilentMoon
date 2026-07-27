using SilentMoon.Application.Features.Pomodoros.Commands.CreatePomodoro;
using SilentMoon.Domain.Entities;
using System;

namespace SilentMoon.Application.Mappings
{
    public static class PomodoroMappingExtensions
    {
        public static Pomodoro ToPomodoro(this CreatePomodoroCommand command,DateTime createDate)
        {
            if (command == null) return null;

            return  new Pomodoro()
            {
                Name = command.Name,
                ShortBreakTime = command.ShortBreakTime,
                LongBreakTime = command.LongBreakTime,
                LongBreakInterval = command.LongBreakInterval,
                PeriodCount = command.PeriodCount,
                Color = command.Color,
                CreateDate = createDate,
                IsDeleted = false,
                PomodoroTime = command.PomodoroTime
            };
        }
    }
}
