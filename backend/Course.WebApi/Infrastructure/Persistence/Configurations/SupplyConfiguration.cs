using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class SupplyConfiguration : IEntityTypeConfiguration<Supply>
{
    public void Configure(EntityTypeBuilder<Supply> builder)
    {
        builder.ToTable("supplies");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SupplierName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(s => s.SupplyDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SupplyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Items)
            .Metadata
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}