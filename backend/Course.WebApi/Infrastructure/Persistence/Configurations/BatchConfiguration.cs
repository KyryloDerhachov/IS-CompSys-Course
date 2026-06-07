using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.QuantityReceived).HasPrecision(18, 3);
        builder.Property(b => b.RemainingQuantity).HasPrecision(18, 3);
        builder.Property(b => b.PurchasePrice).HasPrecision(18, 2);
        
        builder.Property(b => b.Version).IsConcurrencyToken();
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}