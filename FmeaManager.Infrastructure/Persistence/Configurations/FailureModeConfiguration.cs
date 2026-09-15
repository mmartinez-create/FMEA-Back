using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class FailureModeConfiguration : IEntityTypeConfiguration<FailureMode>
{
    public void Configure(EntityTypeBuilder<FailureMode> builder)
    {
        builder.ToTable("FailureModes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProcessStepId).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();

        builder.HasOne<ProcessStep>()
            .WithMany()
            .HasForeignKey(x => x.ProcessStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProcessStepId);
    }
}
