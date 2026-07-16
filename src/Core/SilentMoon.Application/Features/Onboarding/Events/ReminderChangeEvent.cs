using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Events
{
    public class ReminderChangedEvent
    {
        public int ReminderId { get; set; }
        public int UserId { get; set; }
        public string Time { get; set; }
        public int Days { get; set; }
        public bool IsActive { get; set; }
    }
}
