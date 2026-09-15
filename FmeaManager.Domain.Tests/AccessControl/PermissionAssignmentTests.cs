using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Domain.Tests.AccessControl;

public sealed class PermissionAssignmentTests
{
    [Fact]
    public void Create_EditPermission_AlsoGrantsRead()
    {
        var assignment = PermissionAssignment.Create(
            " Engineer@Company.com ",
            PermissionScopeType.Product,
            Guid.NewGuid(),
            AccessPermission.Edit,
            "admin");

        Assert.Equal("engineer@company.com", assignment.UserKey);
        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Read));
        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Edit));
        Assert.False(assignment.Permissions.HasFlag(AccessPermission.Approve));
    }

    [Fact]
    public void Create_ApprovePermission_AlsoGrantsReadButNotEdit()
    {
        var assignment = PermissionAssignment.Create(
            "approver",
            PermissionScopeType.Project,
            Guid.NewGuid(),
            AccessPermission.Approve,
            "admin");

        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Read));
        Assert.False(assignment.Permissions.HasFlag(AccessPermission.Edit));
        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Approve));
    }

    [Fact]
    public void Create_SystemScope_RequiresEmptyGuid()
    {
        Assert.Throws<ArgumentException>(() =>
            PermissionAssignment.Create(
                "admin",
                PermissionScopeType.System,
                Guid.NewGuid(),
                AccessPermission.Approve,
                "admin"));
    }

    [Fact]
    public void UpdatePermissions_ReactivatesAssignment()
    {
        var assignment = PermissionAssignment.Create(
            "user",
            PermissionScopeType.Plant,
            Guid.NewGuid(),
            AccessPermission.Read,
            "admin");

        assignment.Deactivate("admin");
        assignment.UpdatePermissions(
            AccessPermission.Edit,
            "admin");

        Assert.True(assignment.IsActive);
        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Edit));
        Assert.True(assignment.Permissions.HasFlag(AccessPermission.Read));
    }
}
