using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Domain.Errors
{
    public static class ReminderErrors
    {
        public static Error NotFound(int id) => Error.NotFound(
            "Reminders.NotFound",
            $"Reminder with Id = '{id}' was not found.");

        public static readonly Error NotOwned = Error.Forbidden(
            "Reminders.NotOwned",
            "You do not have permission to modify this reminder.");
    }
}
