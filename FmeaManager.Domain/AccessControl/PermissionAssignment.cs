namespace FmeaManager.Domain.AccessControl;

public sealed class PermissionAssignment
{
    private PermissionAssignment()
    {
    }

    private PermissionAssignment(
        Guid id,
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        AccessPermission permissions,
        DateTime createdAt,
        string createdBy)
    {
        Id = id;
        UserKey = NormalizeUserKey(userKey);
        ScopeType = scopeType;
        ScopeId = scopeId;
        Permissions = NormalizePermissions(permissions);
        IsActive = true;
        CreatedAt = createdAt;
        CreatedBy = NormalizeUserKey(createdBy);
    }

    public Guid Id { get; private set; }

    public string UserKey { get; private set; } = string.Empty;

    public PermissionScopeType ScopeType { get; private set; }

    public Guid ScopeId { get; private set; }

    public AccessPermission Permissions { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }

    public static PermissionAssignment Create(
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        AccessPermission permissions,
        string createdBy)
    {
        ValidateScope(scopeType, scopeId);

        if (permissions == AccessPermission.None)
        {
            throw new ArgumentException(
                "At least one permission is required.",
                nameof(permissions));
        }

        return new PermissionAssignment(
            Guid.NewGuid(),
            userKey,
            scopeType,
            scopeId,
            permissions,
            DateTime.UtcNow,
            createdBy);
    }

    public void UpdatePermissions(
        AccessPermission permissions,
        string updatedBy)
    {
        if (permissions == AccessPermission.None)
        {
            throw new ArgumentException(
                "At least one permission is required.",
                nameof(permissions));
        }

        Permissions = NormalizePermissions(permissions);
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = NormalizeUserKey(updatedBy);
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = NormalizeUserKey(updatedBy);
    }

    public bool Grants(AccessPermission permission)
    {
        return IsActive && Permissions.HasFlag(permission);
    }

    private static AccessPermission NormalizePermissions(
        AccessPermission permissions)
    {
        var known =
            AccessPermission.Read |
            AccessPermission.Edit |
            AccessPermission.Approve;

        if ((permissions & ~known) != AccessPermission.None)
        {
            throw new ArgumentOutOfRangeException(
                nameof(permissions),
                permissions,
                "Unknown permission value.");
        }

        // Edit and Approve always require visibility of the resource.
        if (permissions.HasFlag(AccessPermission.Edit) ||
            permissions.HasFlag(AccessPermission.Approve))
        {
            permissions |= AccessPermission.Read;
        }

        return permissions;
    }

    private static void ValidateScope(
        PermissionScopeType scopeType,
        Guid scopeId)
    {
        if (!Enum.IsDefined(scopeType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(scopeType),
                scopeType,
                "Permission scope type is invalid.");
        }

        if (scopeType == PermissionScopeType.System)
        {
            if (scopeId != Guid.Empty)
            {
                throw new ArgumentException(
                    "System scope must use an empty Guid.",
                    nameof(scopeId));
            }

            return;
        }

        if (scopeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Scope id is required.",
                nameof(scopeId));
        }
    }

    private static string NormalizeUserKey(string userKey)
    {
        if (string.IsNullOrWhiteSpace(userKey))
        {
            throw new ArgumentException(
                "User key is required.",
                nameof(userKey));
        }

        return userKey.Trim().ToLowerInvariant();
    }
}
