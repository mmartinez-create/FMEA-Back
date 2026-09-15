using FmeaManager.Domain.Auditing;
using FmeaManager.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ProjectAuditEventConfiguration
    : IEntityTypeConfiguration<ProjectAuditEvent>
{
    public void Configure(
        EntityTypeBuilder<ProjectAuditEvent> builder)
    {
        builder.ToTable("ProjectAuditEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder.Property(x => x.EventName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Action)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityId)
            .IsRequired();

        builder.Property(x => x.ActorUserKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ActorDisplayName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ProjectId,
            x.OccurredAt
        });

        builder.HasIndex(x => x.ActorUserKey);
    }
}
