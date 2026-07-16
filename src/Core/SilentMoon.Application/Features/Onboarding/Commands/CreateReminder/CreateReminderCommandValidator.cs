using FluentValidation;
using SilentMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.CreateReminder
{
    public class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
    {
        private static readonly Regex TimeRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

        public CreateReminderCommandValidator()
        {
            RuleFor(x => x.Time)
                .NotEmpty().WithErrorCode("Reminder.Time.NotEmpty").WithMessage("{PropertyName} is required.")
                .Must(t => t != null && TimeRegex.IsMatch(t))
                    .WithErrorCode("Reminder.Time.Invalid").WithMessage("{PropertyName} must be in HH:mm format (e.g. 08:30).");

            RuleFor(x => x.Days)
                .InclusiveBetween(1, (int)ReminderDays.All)
                    .WithErrorCode("Reminder.Days.Invalid")
                    .WithMessage("{PropertyName} must be a valid day combination (1-127).");
        }
    }
}
