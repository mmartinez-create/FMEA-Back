using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl;

public sealed record EffectiveAccess(
    bool CanRead,
    bool CanEdit,
    bool CanApprove,
    AccessPermission Permissions,
    IReadOnlyList<PermissionSource> Sources)
{
    public static EffectiveAccess None { get; } =
        new(
            false,
            false,
            false,
            AccessPermission.None,
            Array.Empty<PermissionSource>());
}

public sealed record PermissionSource(
    PermissionScopeType ScopeType,
    Guid ScopeId,
    AccessPermission Permissions);
