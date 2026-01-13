using TaskEntity = ChoresHub.Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChoresHub.Infrastructure.EntityConfigs
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("Tasks")
            .HasKey(t => t.Id);

            builder.Property(t => t.Content)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(t => t.IsDone)
            .HasDefaultValue(false)
            .IsRequired();

            builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        }
    }
}