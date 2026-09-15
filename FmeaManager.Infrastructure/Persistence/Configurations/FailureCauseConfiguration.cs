using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class FailureCauseConfiguration : IEntityTypeConfiguration<FailureCause>
{
    public void Configure(EntityTypeBuilder<FailureCause> builder)
    {
        builder.ToTable("FailureCauses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FailureModeId).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();

        builder.HasOne<FailureMode>()
            .WithMany()
            .HasForeignKey(x => x.FailureModeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.FailureModeId);
    }
}
