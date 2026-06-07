using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("receipts");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReceiptNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(r => r.ReceiptNumber)
            .IsUnique();

        builder.Property(r => r.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(r => r.Version)
            .IsConcurrencyToken();

        builder.HasMany(r => r.Items)
            .WithOne()
            .HasForeignKey(i => i.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Items)
            .Metadata
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}