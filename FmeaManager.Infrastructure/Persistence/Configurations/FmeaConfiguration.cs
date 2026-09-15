using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class FmeaConfiguration : IEntityTypeConfiguration<Fmea>
{
    public void Configure(EntityTypeBuilder<Fmea> builder)
    {
        builder.ToTable("Fmeas");

        builder.HasKey(fmea => fmea.Id);

        builder.Property(fmea => fmea.ProjectId)
            .IsRequired();

        builder.Property(fmea => fmea.Number)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(fmea => fmea.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(fmea => fmea.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fmea => fmea.Owner)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(fmea => fmea.IsActive)
            .IsRequired();

        builder.Property(fmea => fmea.CreatedAt)
            .IsRequired();

        builder.Property(fmea => fmea.CreatedBy)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(fmea => fmea.UpdatedAt);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(fmea => fmea.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(fmea => new
        {
            fmea.ProjectId,
            fmea.Number
        })
        .IsUnique();

        builder.HasIndex(fmea => fmea.ProjectId);
    }
}
