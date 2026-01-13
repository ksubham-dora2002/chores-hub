using ChoresHub.Domain.Entities;
using TaskEntity = ChoresHub.Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;

namespace ChoresHub.Infrastructure.Contexts
{
    public class PostgreDbContext(DbContextOptions<PostgreDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ShoppingProduct> ShoppingProducts { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration<T> in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreDbContext).Assembly);

            // Alternatively, apply configurations individually
            // modelBuilder.ApplyConfiguration(new ChoresHub.Infrastructure.EntityConfigurations.UserConfiguration());
            // modelBuilder.ApplyConfiguration(new ChoresHub.Infrastructure.EntityConfigurations.NotificationConfiguration());
            // modelBuilder.ApplyConfiguration(new ChoresHub.Infrastructure.EntityConfigurations.TaskConfiguration());
            // modelBuilder.ApplyConfiguration(new ChoresHub.Infrastructure.EntityConfigurations.ShoppingProductConfiguration());
            // modelBuilder.ApplyConfiguration(new ChoresHub.Infrastructure.EntityConfigurations.RefreshTokenConfiguration());

            base.OnModelCreating(modelBuilder);

        }
    }
}