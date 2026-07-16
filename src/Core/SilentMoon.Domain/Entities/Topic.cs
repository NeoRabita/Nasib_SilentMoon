using SilentMoon.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Domain.Entities
{

    public class Topic : BaseEntity
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }

        public virtual ICollection<UserTopic> UserTopics { get; set; } = new HashSet<UserTopic>();
    }
}
