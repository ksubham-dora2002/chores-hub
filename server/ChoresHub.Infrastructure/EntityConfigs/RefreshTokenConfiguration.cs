using ChoresHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChoresHub.Infrastructure.EntityConfigs
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens")
            .HasKey(rf => rf.Id);

            builder.Property(rf => rf.UserEmail)
            .IsRequired();

            builder.Property(rf => rf.Token)
            .IsRequired()
            .HasMaxLength(512);

            builder.Property(rf => rf.ExpiresAt)
            .IsRequired();

            builder.Property(rf => rf.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(rf => rf.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        }
    }
}