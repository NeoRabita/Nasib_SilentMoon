using FluentValidation;
using SilentMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.UpdateReminder
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        private static readonly Regex TimeRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

        public UpdateReminderCommandValidator()
        {
            RuleFor(x => x.Time)
                .Must(t => TimeRegex.IsMatch(t))
                    .WithErrorCode("Reminder.Time.Invalid").WithMessage("{PropertyName} must be in HH:mm format (e.g. 08:30).")
                .When(x => x.Time is not null);

            RuleFor(x => x.Days)
                .InclusiveBetween(1, (int)ReminderDays.All)
                    .WithErrorCode("Reminder.Days.Invalid")
                    .WithMessage("{PropertyName} must be a valid day combination (1-127).")
                .When(x => x.Days.HasValue);
        }
    }
}
