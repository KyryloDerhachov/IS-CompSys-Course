using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
        builder.Property(p => p.Unit).HasMaxLength(20).HasDefaultValue("шт");

        builder.Property(p => p.Attributes)
        .HasColumnType("jsonb")
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)
                ?? new Dictionary<string, string>());

        builder.Property(p => p.Version).IsConcurrencyToken();
    }
}