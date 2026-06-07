using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class ProductClassConfiguration : IEntityTypeConfiguration<ProductClass>
{
    public void Configure(EntityTypeBuilder<ProductClass> builder)
    {
        builder.ToTable("product_classes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.Version)
            .IsConcurrencyToken();
    }
}