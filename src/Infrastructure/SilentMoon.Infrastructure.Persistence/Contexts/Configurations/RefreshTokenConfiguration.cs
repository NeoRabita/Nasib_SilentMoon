using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SilentMoon.Domain.Entities;

namespace SilentMoon.Infrastructure.Persistence.Contexts.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token).HasMaxLength(200).IsRequired();

            builder.HasIndex(rt => rt.Token).IsUnique();

            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}