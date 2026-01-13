using ChoresHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChoresHub.Infrastructure.EntityConfigs
{
    public class ShoppingProductConfiguration : IEntityTypeConfiguration<ShoppingProduct>
    {
        public void Configure(EntityTypeBuilder<ShoppingProduct> builder)
        {
            builder.ToTable("ShoppingProducts")
            .HasKey(sp => sp.Id);

            builder.Property(sp => sp.Content)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(sp => sp.IsBought)
            .HasDefaultValue(false)
            .IsRequired();

            builder.Property(sp => sp.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}