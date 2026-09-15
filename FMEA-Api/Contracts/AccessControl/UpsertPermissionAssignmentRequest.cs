using FmeaManager.Domain.AccessControl;

namespace FMEA_Api.Contracts.AccessControl;

public sealed record UpsertPermissionAssignmentRequest(
    PermissionScopeType ScopeType,
    Guid ScopeId,
    bool CanRead,
    bool CanEdit,
    bool CanApprove);
