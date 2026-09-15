using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Auditing;

public sealed class GetLastProjectActivityHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectAuditRepository _auditRepository;

    public GetLastProjectActivityHandler(
        IProjectRepository projectRepository,
        IProjectAuditRepository auditRepository)
    {
        _projectRepository = projectRepository;
        _auditRepository = auditRepository;
    }

    public async Task<LastProjectActivityResult?> HandleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(
            projectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{projectId}' was not found.");
        }

        var auditEvent = await _auditRepository.GetLastByProjectIdAsync(
            projectId,
            cancellationToken);

        return auditEvent is null
            ? null
            : new LastProjectActivityResult(
                auditEvent.Id,
                auditEvent.ProjectId,
                auditEvent.EventName,
                auditEvent.Action,
                auditEvent.EntityType,
                auditEvent.EntityId,
                auditEvent.ActorUserKey,
                auditEvent.ActorDisplayName,
                auditEvent.OccurredAt);
    }
}
