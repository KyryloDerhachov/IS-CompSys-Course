using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class ReturnTransactionConfiguration : IEntityTypeConfiguration<ReturnTransaction>
{
    public void Configure(EntityTypeBuilder<ReturnTransaction> builder)
    {
        builder.ToTable("return_transactions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReturnNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(r => r.ReturnNumber)
            .IsUnique();

        builder.Property(r => r.TotalRefundAmount)
            .HasPrecision(18, 2);

        builder.Property(r => r.Version)
            .IsConcurrencyToken();

        builder.HasOne<Receipt>()
            .WithMany()
            .HasForeignKey(r => r.ReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Items)
            .WithOne()
            .HasForeignKey(i => i.ReturnTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Items)
            .Metadata
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}