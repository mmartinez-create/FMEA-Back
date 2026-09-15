using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl.UpsertPermissionAssignment;

public sealed class UpsertPermissionAssignmentHandler
{
    private readonly IPermissionAssignmentRepository _repository;
    private readonly AccessControlService _accessControl;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertPermissionAssignmentHandler(
        IPermissionAssignmentRepository repository,
        AccessControlService accessControl,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _accessControl = accessControl;
        _unitOfWork = unitOfWork;
    }

    public async Task<PermissionAssignmentResult> HandleAsync(
        UpsertPermissionAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var changerAccess = await _accessControl.GetScopeAccessAsync(
            command.ChangedBy,
            command.ScopeType,
            command.ScopeId,
            cancellationToken);

        if (!changerAccess.CanApprove)
        {
            throw new ForbiddenException(
                "Approve permission is required to manage access at this scope.");
        }

        var permissions = AccessPermission.None;

        if (command.CanRead)
        {
            permissions |= AccessPermission.Read;
        }

        if (command.CanEdit)
        {
            permissions |= AccessPermission.Edit;
        }

        if (command.CanApprove)
        {
            permissions |= AccessPermission.Approve;
        }

        if (permissions == AccessPermission.None)
        {
            throw new ArgumentException(
                "At least one permission must be selected.");
        }

        var assignment = await _repository.GetForUpdateAsync(
            command.UserKey.Trim().ToLowerInvariant(),
            command.ScopeType,
            command.ScopeId,
            cancellationToken);

        if (assignment is null)
        {
            assignment = PermissionAssignment.Create(
                command.UserKey,
                command.ScopeType,
                command.ScopeId,
                permissions,
                command.ChangedBy);

            _repository.Add(assignment);
        }
        else
        {
            assignment.UpdatePermissions(
                permissions,
                command.ChangedBy);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(assignment);
    }

    private static PermissionAssignmentResult Map(
        PermissionAssignment assignment)
    {
        return new PermissionAssignmentResult(
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
            assignment.UpdatedBy);
    }
}
