using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ControlPlanConfiguration : IEntityTypeConfiguration<ControlPlan>
{
    public void Configure(EntityTypeBuilder<ControlPlan> builder)
    {
        builder.ToTable("ControlPlans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProjectId).IsRequired();
        builder.Property(x => x.Number).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ApprovedBy).HasMaxLength(256);
        builder.Property(x => x.RejectedBy).HasMaxLength(256);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);

        builder.Ignore(x => x.IsEditable);

        builder.HasOne<Project>()
            .WithOne()
            .HasForeignKey<ControlPlan>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProjectId).IsUnique();
        builder.HasIndex(x => x.Number);
    }
}
