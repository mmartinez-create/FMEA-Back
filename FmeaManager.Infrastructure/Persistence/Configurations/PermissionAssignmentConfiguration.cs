using FmeaManager.Domain.AccessControl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FmeaManager.Infrastructure.Persistence.Configurations;

public sealed class PermissionAssignmentConfiguration
    : IEntityTypeConfiguration<PermissionAssignment>
{
    private static readonly Guid DevelopmentAdminAssignmentId =
        Guid.Parse("4b1f9ee8-97f2-4f4a-94af-5fe5d08b6af7");

    public void Configure(
        EntityTypeBuilder<PermissionAssignment> builder)
    {
        builder.ToTable("PermissionAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ScopeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ScopeId)
            .IsRequired();

        builder.Property(x => x.Permissions)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.HasIndex(x => new
        {
            x.UserKey,
            x.ScopeType,
            x.ScopeId
        })
        .IsUnique();

        builder.HasData(new
        {
            Id = DevelopmentAdminAssignmentId,
            UserKey = "local-development",
            ScopeType = PermissionScopeType.System,
            ScopeId = Guid.Empty,
            Permissions =
                AccessPermission.Read |
                AccessPermission.Edit |
                AccessPermission.Approve,
            IsActive = true,
            CreatedAt = new DateTime(
                2026, 9, 10, 12, 0, 0,
                DateTimeKind.Utc),
            CreatedBy = "system-bootstrap",
            UpdatedAt = (DateTime?)null,
            UpdatedBy = (string?)null
        });
    }
}
