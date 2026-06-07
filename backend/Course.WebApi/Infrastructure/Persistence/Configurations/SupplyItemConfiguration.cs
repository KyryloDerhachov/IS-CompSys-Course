using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class SupplyItemConfiguration : IEntityTypeConfiguration<SupplyItem>
{
    public void Configure(EntityTypeBuilder<SupplyItem> builder)
    {
        builder.ToTable("supply_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(i => i.PurchasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.ShelfLifeDays)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}