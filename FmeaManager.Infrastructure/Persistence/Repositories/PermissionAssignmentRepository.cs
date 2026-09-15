using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class PermissionAssignmentRepository
    : IPermissionAssignmentRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public PermissionAssignmentRepository(
        FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PermissionAssignment?> GetForUpdateAsync(
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.PermissionAssignments
            .SingleOrDefaultAsync(
                assignment =>
                    assignment.UserKey == userKey &&
                    assignment.ScopeType == scopeType &&
                    assignment.ScopeId == scopeId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionAssignment>>
        ListActiveByUserAsync(
            string userKey,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.PermissionAssignments
            .AsNoTracking()
            .Where(assignment =>
                assignment.UserKey == userKey &&
                assignment.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionAssignment>>
        ListByUserAsync(
            string userKey,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.PermissionAssignments
            .AsNoTracking()
            .Where(assignment => assignment.UserKey == userKey)
            .OrderBy(assignment => assignment.ScopeType)
            .ThenBy(assignment => assignment.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(PermissionAssignment assignment)
    {
        _dbContext.PermissionAssignments.Add(assignment);
    }
}
