using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Contexts.Configurations
{
    public class UserTopicConfiguration : IEntityTypeConfiguration<UserTopic>
    {
        public void Configure(EntityTypeBuilder<UserTopic> builder)
        {
            builder.ToTable("UserTopics");
            builder.HasKey(ut => ut.Id);

          
            builder.HasIndex(ut => new { ut.UserId, ut.TopicId }).IsUnique();

            builder.HasOne(ut => ut.User)
                   .WithMany(u => u.UserTopics)
                   .HasForeignKey(ut => ut.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ut => ut.Topic)
                   .WithMany(t => t.UserTopics)
                   .HasForeignKey(ut => ut.TopicId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
