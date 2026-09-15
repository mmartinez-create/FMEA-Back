using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Auditing;

namespace FmeaManager.Application.Auditing;

public sealed class ProjectAuditService
{
    private readonly IProjectAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IFailureModeRepository _failureModeRepository;
    private readonly IFailureCauseRepository _failureCauseRepository;

    public ProjectAuditService(
        IProjectAuditRepository auditRepository,
        IUnitOfWork unitOfWork,
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository,
        IProcessStepRepository processStepRepository,
        IFailureModeRepository failureModeRepository,
        IFailureCauseRepository failureCauseRepository)
    {
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
        _processStepRepository = processStepRepository;
        _failureModeRepository = failureModeRepository;
        _failureCauseRepository = failureCauseRepository;
    }

    public Task RecordForProjectAsync(
        Guid projectId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        return RecordAsync(
            projectId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    public async Task RecordForFmeaAsync(
        Guid fmeaId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        var fmea = await _fmeaRepository.GetByIdAsync(
            fmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{fmeaId}' was not found.");
        }

        await RecordAsync(
            fmea.ProjectId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    public async Task RecordForRevisionAsync(
        Guid revisionId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        var revision = await _revisionRepository.GetByIdAsync(
            revisionId,
            cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{revisionId}' was not found.");
        }

        await RecordForFmeaAsync(
            revision.FmeaId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    public async Task RecordForProcessStepAsync(
        Guid processStepId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        var processStep = await _processStepRepository.GetByIdAsync(
            processStepId,
            cancellationToken);

        if (processStep is null)
        {
            throw new NotFoundException(
                $"Process step '{processStepId}' was not found.");
        }

        await RecordForRevisionAsync(
            processStep.FmeaRevisionId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    public async Task RecordForFailureModeAsync(
        Guid failureModeId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        var failureMode = await _failureModeRepository.GetByIdAsync(
            failureModeId,
            cancellationToken);

        if (failureMode is null)
        {
            throw new NotFoundException(
                $"Failure mode '{failureModeId}' was not found.");
        }

        await RecordForProcessStepAsync(
            failureMode.ProcessStepId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    public async Task RecordForFailureCauseAsync(
        Guid failureCauseId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken = default)
    {
        var failureCause = await _failureCauseRepository.GetByIdAsync(
            failureCauseId,
            cancellationToken);

        if (failureCause is null)
        {
            throw new NotFoundException(
                $"Failure cause '{failureCauseId}' was not found.");
        }

        await RecordForFailureModeAsync(
            failureCause.FailureModeId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName,
            cancellationToken);
    }

    private async Task RecordAsync(
        Guid projectId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        CancellationToken cancellationToken)
    {
        var auditEvent = ProjectAuditEvent.Create(
            projectId,
            eventName,
            action,
            entityType,
            entityId,
            actorUserKey,
            actorDisplayName);

        _auditRepository.Add(auditEvent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
