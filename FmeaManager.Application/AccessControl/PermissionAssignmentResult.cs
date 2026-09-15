using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl;

public sealed record PermissionAssignmentResult(
    Guid Id,
    string UserKey,
    PermissionScopeType ScopeType,
    Guid ScopeId,
    bool CanRead,
    bool CanEdit,
    bool CanApprove,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy);
