using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Mappings
{
    public static class ProfileMappingExtensions
    {
        public static ProfileViewModel ToProfileViewModel(this ApplicationUser user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            IsEmailVerified = user.IsEmailVerified,
            AuthenticationProvider = user.AuthenticationProvider.ToString()
        };

        public static TopicViewModel ToTopicViewModel(this Topic topic) => new()
        {
            Id = topic.Id,
            Name = topic.Name,
            IconUrl = topic.IconUrl,
            Color = topic.Color
        };

        public static ReminderViewModel ToReminderViewModel(this Reminder reminder) => new()
        {
            Id = reminder.Id,
            Time = reminder.Time,
            Days = reminder.Days,
            IsActive = reminder.IsActive,
            DayNames = Enum.GetValues<ReminderDays>()
                .Where(d => d != ReminderDays.None
                         && d != ReminderDays.Weekdays
                         && d != ReminderDays.Weekend
                         && d != ReminderDays.All
                         && ((ReminderDays)reminder.Days).HasFlag(d))
                .Select(d => d.ToString())
                .ToArray()
        };
    }
}
