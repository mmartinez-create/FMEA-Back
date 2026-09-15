using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IPermissionAssignmentRepository
{
    Task<PermissionAssignment?> GetForUpdateAsync(
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PermissionAssignment>> ListActiveByUserAsync(
        string userKey,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PermissionAssignment>> ListByUserAsync(
        string userKey,
        CancellationToken cancellationToken = default);

    void Add(PermissionAssignment assignment);
}
