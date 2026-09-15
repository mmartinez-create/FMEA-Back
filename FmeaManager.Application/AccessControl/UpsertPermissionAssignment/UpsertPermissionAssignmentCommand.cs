using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl.UpsertPermissionAssignment;

public sealed record UpsertPermissionAssignmentCommand(
    string UserKey,
    PermissionScopeType ScopeType,
    Guid ScopeId,
    bool CanRead,
    bool CanEdit,
    bool CanApprove,
    string ChangedBy);
