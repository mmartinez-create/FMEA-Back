using FmeaManager.Domain.CustomerProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class CustomerProfileConfiguration
    : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("CustomerProfiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(profile => profile.Code)
            .IsUnique();

        builder.Property(profile => profile.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(profile => profile.Description)
            .HasMaxLength(500);

        builder.Property(profile => profile.IsActive)
            .IsRequired();

        builder.HasData(
            new
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Code = "STANDARD",
                Name = "Standard FMEA",
                Description = "Default customer-independent FMEA profile.",
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Code = "FORD",
                Name = "Ford",
                Description = "Initial Ford customer profile. Customer-specific rules will be refined in later increments.",
                IsActive = true
            });
    }
}
