using Bcp.Exchange.Core.ExchangeRates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bcp.Exchange.Infrastructure.Persistence.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("ExchangeRates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Rate).IsRequired().HasPrecision(18, 6);

        builder.Property(e => e.CurrencySourceId).IsRequired();

        builder.Property(e => e.CurrencyTargetId).IsRequired();

        builder.Property(e => e.IsActive).IsRequired();

        builder.Property(e => e.CreatedAt).IsRequired();

        builder.Property(e => e.CreatedBy).HasMaxLength(100);

        builder.Property(e => e.ModifiedBy).HasMaxLength(100);

        builder
            .HasOne(e => e.CurrencySource)
            .WithMany()
            .HasForeignKey(e => e.CurrencySourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(e => e.CurrencyTarget)
            .WithMany()
            .HasForeignKey(e => e.CurrencyTargetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(e => new
            {
                e.CurrencySourceId,
                e.CurrencyTargetId,
                e.IsActive,
            })
            .HasFilter("[IsActive] = 1");

        builder.HasQueryFilter(e => e.IsActive);
    }
}
