using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Auditing;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProjectAuditRepository
    : IProjectAuditRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProjectAuditRepository(
        FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProjectAuditEvent?> GetLastByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProjectAuditEvents
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectAuditEvent>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProjectAuditEvents
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public void Add(ProjectAuditEvent auditEvent)
    {
        _dbContext.ProjectAuditEvents.Add(auditEvent);
    }
}
