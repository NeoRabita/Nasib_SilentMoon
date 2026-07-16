using SilentMoon.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Domain.Entities
{
    public class UserTopic : BaseEntity
    {
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        public DateTime SelectedAt { get; set; } = DateTime.UtcNow;
    }
}
