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
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topics");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
            builder.Property(t => t.IconUrl).HasMaxLength(500).IsRequired(false);
            builder.Property(t => t.Color).HasMaxLength(20).IsRequired(false);

            builder.HasIndex(t => t.Name).IsUnique();

           
            builder.HasData(
                new Topic { Id = 1, Name = "Reduce Stress", Color = "#8E97FD", SortOrder = 1, IsActive = true },
                new Topic { Id = 2, Name = "Improve Performance", Color = "#FA6E5A", SortOrder = 2, IsActive = true },
                new Topic { Id = 3, Name = "Increase Happiness", Color = "#FEB18F", SortOrder = 3, IsActive = true },
                new Topic { Id = 4, Name = "Reduce Anxiety", Color = "#FFCF86", SortOrder = 4, IsActive = true },
                new Topic { Id = 5, Name = "Personal Growth", Color = "#6CB28E", SortOrder = 5, IsActive = true },
                new Topic { Id = 6, Name = "Better Sleep", Color = "#3F414E", SortOrder = 6, IsActive = true }
            );
        }
    }
}
