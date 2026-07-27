using System;
using System.ComponentModel.DataAnnotations.Schema;
using SilentMoon.Domain.Common;

namespace SilentMoon.Domain.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}