using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ControlPlanItemConfiguration : IEntityTypeConfiguration<ControlPlanItem>
{
    public void Configure(EntityTypeBuilder<ControlPlanItem> builder)
    {
        builder.ToTable("ControlPlanItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProcessStepNameSnapshot).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CharacteristicNumber).HasMaxLength(80);
        builder.Property(x => x.CharacteristicType).IsRequired();
        builder.Property(x => x.CharacteristicName).HasMaxLength(300).IsRequired();
        builder.Property(x => x.MachineTooling).HasMaxLength(300);
        builder.Property(x => x.SpecialCharacteristic).HasMaxLength(80);
        builder.Property(x => x.SpecificationTolerance).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.EvaluationMeasurementTechnique).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.SampleFrequency).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ControlMethod).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ReactionPlan).HasMaxLength(1500).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(256).IsRequired();
        builder.Property(x => x.UpdatedBy).HasMaxLength(256);

        builder.HasOne<ControlPlan>()
            .WithMany()
            .HasForeignKey(x => x.ControlPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductProcessStep>()
            .WithMany()
            .HasForeignKey(x => x.ProductProcessStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ControlPlanId);
        builder.HasIndex(x => x.ProductProcessStepId);
        builder.HasIndex(x => new { x.ControlPlanId, x.ProductProcessStepId });
    }
}
