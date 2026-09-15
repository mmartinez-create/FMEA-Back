using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ProductProcessStepConfiguration
    : IEntityTypeConfiguration<ProductProcessStep>
{
    public void Configure(EntityTypeBuilder<ProductProcessStep> builder)
    {
        builder.ToTable("ProductProcessSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductProcessId)
            .IsRequired();

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Function)
            .HasMaxLength(1000);

        builder.Property(x => x.Requirement)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.HasOne<ProductProcess>()
            .WithMany()
            .HasForeignKey(x => x.ProductProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ProductProcessId,
            x.Sequence
        })
        .IsUnique();
    }
}
