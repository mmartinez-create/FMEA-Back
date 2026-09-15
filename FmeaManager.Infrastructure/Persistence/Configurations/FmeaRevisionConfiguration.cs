using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class FmeaRevisionConfiguration
    : IEntityTypeConfiguration<FmeaRevision>
{
    public void Configure(EntityTypeBuilder<FmeaRevision> builder)
    {
        builder.ToTable("FmeaRevisions");

        builder.HasKey(revision => revision.Id);

        builder.Property(revision => revision.FmeaId)
            .IsRequired();

        builder.Property(revision => revision.RevisionNumber)
            .IsRequired();

        builder.Ignore(revision => revision.RevisionCode);

        builder.Property(revision => revision.BasedOnRevisionId);

        builder.Property(revision => revision.RevisionReason)
            .HasMaxLength(500);

        builder.Property(revision => revision.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(revision => revision.CreatedAt)
            .IsRequired();

        builder.Property(revision => revision.CreatedBy)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(revision => revision.SubmittedAt);

        builder.Property(revision => revision.ApprovedAt);

        builder.Property(revision => revision.ApprovedBy)
            .HasMaxLength(150);

        builder.Property(revision => revision.RejectedAt);

        builder.Property(revision => revision.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne<Fmea>()
            .WithMany()
            .HasForeignKey(revision => revision.FmeaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<FmeaRevision>()
            .WithMany()
            .HasForeignKey(revision => revision.BasedOnRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(revision => new
        {
            revision.FmeaId,
            revision.RevisionNumber
        })
        .IsUnique();

        builder.HasIndex(revision => revision.FmeaId);

        builder.HasIndex(revision => revision.BasedOnRevisionId);
    }
}
