using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.AccessControl.RevokePermissionAssignment;

public sealed class RevokePermissionAssignmentHandler
{
    private readonly IPermissionAssignmentRepository _repository;
    private readonly AccessControlService _accessControl;
    private readonly IUnitOfWork _unitOfWork;

    public RevokePermissionAssignmentHandler(
        IPermissionAssignmentRepository repository,
        AccessControlService accessControl,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _accessControl = accessControl;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        RevokePermissionAssignmentCommand command,
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
                "Approve permission is required to revoke access at this scope.");
        }

        var assignment = await _repository.GetForUpdateAsync(
            command.UserKey.Trim().ToLowerInvariant(),
            command.ScopeType,
            command.ScopeId,
            cancellationToken);

        if (assignment is null)
        {
            throw new NotFoundException(
                "The permission assignment was not found.");
        }

        assignment.Deactivate(command.ChangedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
