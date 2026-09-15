using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class DetectionControlConfiguration : IEntityTypeConfiguration<DetectionControl>
{
    public void Configure(EntityTypeBuilder<DetectionControl> builder)
    {
        builder.ToTable("DetectionControls");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FailureCauseId).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();

        builder.HasOne<FailureCause>()
            .WithMany()
            .HasForeignKey(x => x.FailureCauseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.FailureCauseId);
    }
}
