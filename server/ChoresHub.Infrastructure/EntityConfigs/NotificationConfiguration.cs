using ChoresHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChoresHub.Infrastructure.EntityConfigs
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications")
            .HasKey(n => n.Id);

            builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(n => n.IsSeen)
            .IsRequired()
            .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}