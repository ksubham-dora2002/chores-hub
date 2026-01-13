using ChoresHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChoresHub.Infrastructure.EntityConfigs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users")
            .HasKey(u => u.Id);

            builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

            builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(250);

            builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(200);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Tasks)
                   .WithOne(t => t.User)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ShoppingProducts)
                   .WithOne(sp => sp.User)
                   .HasForeignKey(sp => sp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(rf => rf.User)
                   .HasForeignKey(rf => rf.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}