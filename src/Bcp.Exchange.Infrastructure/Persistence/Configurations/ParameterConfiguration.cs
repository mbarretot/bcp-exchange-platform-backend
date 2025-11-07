using Bcp.Exchange.Core.Configuration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bcp.Exchange.Infrastructure.Persistence.Configurations;

public class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
{
    public void Configure(EntityTypeBuilder<Parameter> builder)
    {
        builder.ToTable("Parameters");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Description).IsRequired().HasMaxLength(200);

        builder.Property(p => p.LongDescription).HasMaxLength(500);

        builder.Property(p => p.DisplayOrder).IsRequired();

        builder.Property(p => p.NumericValue).HasPrecision(18, 4);

        builder.Property(p => p.TextValue).HasMaxLength(500);

        builder.Property(p => p.IsActive).IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();

        builder.Property(p => p.CreatedBy).HasMaxLength(100);

        builder.Property(p => p.ModifiedBy).HasMaxLength(100);

        builder
            .HasOne<Parameter>()
            .WithMany()
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
