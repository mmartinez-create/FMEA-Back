using FmeaManager.Domain.ProductStructure;
using FmeaManager.Domain.CustomerProfiles;
using FmeaManager.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(project => project.Code)
            .IsUnique();

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Product)
            .HasMaxLength(200);

        builder.Property(project => project.Plant)
            .HasMaxLength(150);

        builder.Property(project => project.Owner)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(project => project.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(project => project.CreatedAt)
            .IsRequired();

        builder.Property(project => project.CreatedBy)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(project => project.UpdatedAt);

        builder.HasOne<CustomerProfile>()
            .WithMany()
            .HasForeignKey(project => project.CustomerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(project => project.CustomerProfileId);


        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(project => project.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(project => project.ProductId);

    }
}
