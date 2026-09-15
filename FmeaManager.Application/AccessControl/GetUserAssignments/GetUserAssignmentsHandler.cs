using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl.GetUserAssignments;

public sealed class GetUserAssignmentsHandler
{
    private readonly IPermissionAssignmentRepository _repository;
    private readonly AccessControlService _accessControl;

    public GetUserAssignmentsHandler(
        IPermissionAssignmentRepository repository,
        AccessControlService accessControl)
    {
        _repository = repository;
        _accessControl = accessControl;
    }

    public async Task<IReadOnlyList<PermissionAssignmentResult>> HandleAsync(
        string requestedUserKey,
        string currentUserKey,
        CancellationToken cancellationToken = default)
    {
        var requestedNormalized =
            requestedUserKey.Trim().ToLowerInvariant();

        var currentNormalized =
            currentUserKey.Trim().ToLowerInvariant();

        var assignments = await _repository.ListByUserAsync(
            requestedNormalized,
            cancellationToken);

        var visible = new List<PermissionAssignmentResult>();

        foreach (var assignment in assignments)
        {
            var currentAccess = await _accessControl.GetScopeAccessAsync(
                currentNormalized,
                assignment.ScopeType,
                assignment.ScopeId,
                cancellationToken);

            if (!currentAccess.CanApprove &&
                requestedNormalized != currentNormalized)
            {
                continue;
            }

            visible.Add(new PermissionAssignmentResult(
                assignment.Id,
                assignment.UserKey,
                assignment.ScopeType,
                assignment.ScopeId,
                assignment.Permissions.HasFlag(AccessPermission.Read),
                assignment.Permissions.HasFlag(AccessPermission.Edit),
                assignment.Permissions.HasFlag(AccessPermission.Approve),
                assignment.IsActive,
                assignment.CreatedAt,
                assignment.CreatedBy,
                assignment.UpdatedAt,
                assignment.UpdatedBy));
        }

        return visible;
    }
}
