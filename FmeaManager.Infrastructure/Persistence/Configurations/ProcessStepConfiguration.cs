using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ProcessStepConfiguration
    : IEntityTypeConfiguration<ProcessStep>
{
    public void Configure(EntityTypeBuilder<ProcessStep> builder)
    {
        builder.ToTable("ProcessSteps");

        builder.HasKey(step => step.Id);

        builder.Property(step => step.FmeaRevisionId)
            .IsRequired();

        builder.Property(step => step.ProductProcessStepId);

        builder.Property(step => step.Sequence)
            .IsRequired();

        builder.Property(step => step.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(step => step.Function)
            .HasMaxLength(1000);

        builder.Property(step => step.Requirement)
            .HasMaxLength(1000);

        builder.HasOne<FmeaRevision>()
            .WithMany()
            .HasForeignKey(step => step.FmeaRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductProcessStep>()
            .WithMany()
            .HasForeignKey(step => step.ProductProcessStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(step => new
        {
            step.FmeaRevisionId,
            step.Sequence
        })
        .IsUnique();

        builder.HasIndex(step => step.FmeaRevisionId);

        builder.HasIndex(step => step.ProductProcessStepId);
    }
}
