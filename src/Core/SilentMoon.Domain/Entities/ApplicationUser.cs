using SilentMoon.Domain.Common;
using SilentMoon.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SilentMoon.Domain.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public ApplicationUser()
        {
            Pomodoros = new HashSet<Pomodoro>();
            RefreshTokens = new HashSet<RefreshToken>();
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public AuthenticationProvider AuthenticationProvider { get; set; } = AuthenticationProvider.Local;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        public virtual ICollection<Pomodoro> Pomodoros { get; set; }
    }

}
