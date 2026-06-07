using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.WebApi.Infrastructure.Persistence.Configurations;

public class FeedbackRecordConfiguration : IEntityTypeConfiguration<FeedbackRecord>
{
    public void Configure(EntityTypeBuilder<FeedbackRecord> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(f => f.ReceiptNumber)
            .IsUnique();

        builder.Property(f => f.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(f => f.ManagerResponse)
            .HasMaxLength(2000);

        builder.Property(f => f.Type)
            .IsRequired()
            .HasConversion<int>();
    }
}