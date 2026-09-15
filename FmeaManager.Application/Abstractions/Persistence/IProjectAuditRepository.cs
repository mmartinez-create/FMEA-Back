using FmeaManager.Domain.Auditing;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProjectAuditRepository
{
    Task<ProjectAuditEvent?> GetLastByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectAuditEvent>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    void Add(ProjectAuditEvent auditEvent);
}
