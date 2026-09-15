using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class RiskAssessmentConfiguration
    : IEntityTypeConfiguration<RiskAssessment>
{
    public void Configure(EntityTypeBuilder<RiskAssessment> builder)
    {
        builder.ToTable(
            "RiskAssessments",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_RiskAssessments_Severity",
                    "[Severity] BETWEEN 1 AND 10");

                table.HasCheckConstraint(
                    "CK_RiskAssessments_Occurrence",
                    "[Occurrence] BETWEEN 1 AND 10");

                table.HasCheckConstraint(
                    "CK_RiskAssessments_Detection",
                    "[Detection] BETWEEN 1 AND 10");

                table.HasCheckConstraint(
                    "CK_RiskAssessments_Rpn",
                    "[Rpn] = [Severity] * [Occurrence] * [Detection]");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FailureCauseId)
            .IsRequired();

        builder.Property(x => x.Severity)
            .IsRequired();

        builder.Property(x => x.Occurrence)
            .IsRequired();

        builder.Property(x => x.Detection)
            .IsRequired();

        builder.Property(x => x.Rpn)
            .IsRequired();

        builder.Ignore(x => x.ActionPriority);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(150);

        builder.HasOne<FailureCause>()
            .WithOne()
            .HasForeignKey<RiskAssessment>(x => x.FailureCauseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.FailureCauseId)
            .IsUnique();
    }
}
