using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl.RevokePermissionAssignment;

public sealed record RevokePermissionAssignmentCommand(
    string UserKey,
    PermissionScopeType ScopeType,
    Guid ScopeId,
    string ChangedBy);
