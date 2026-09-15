using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.Common;

public sealed class FmeaEditabilityGuard
{
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IFailureModeRepository _failureModeRepository;
    private readonly IFailureCauseRepository _failureCauseRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;

    public FmeaEditabilityGuard(
        IProcessStepRepository processStepRepository,
        IFailureModeRepository failureModeRepository,
        IFailureCauseRepository failureCauseRepository,
        IFmeaRevisionRepository revisionRepository)
    {
        _processStepRepository = processStepRepository;
        _failureModeRepository = failureModeRepository;
        _failureCauseRepository = failureCauseRepository;
        _revisionRepository = revisionRepository;
    }

    public async Task<ProcessStep> EnsureProcessStepEditableAsync(
        Guid processStepId,
        CancellationToken cancellationToken = default)
    {
        var processStep = await _processStepRepository.GetByIdAsync(processStepId, cancellationToken);

        if (processStep is null)
        {
            throw new NotFoundException($"Process step '{processStepId}' was not found.");
        }

        await EnsureRevisionEditableAsync(processStep.FmeaRevisionId, cancellationToken);
        return processStep;
    }

    public async Task<FailureMode> EnsureFailureModeEditableAsync(
        Guid failureModeId,
        CancellationToken cancellationToken = default)
    {
        var failureMode = await _failureModeRepository.GetByIdAsync(failureModeId, cancellationToken);

        if (failureMode is null)
        {
            throw new NotFoundException($"Failure mode '{failureModeId}' was not found.");
        }

        await EnsureProcessStepEditableAsync(failureMode.ProcessStepId, cancellationToken);
        return failureMode;
    }

    public async Task<FailureCause> EnsureFailureCauseEditableAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        var failureCause = await _failureCauseRepository.GetByIdAsync(failureCauseId, cancellationToken);

        if (failureCause is null)
        {
            throw new NotFoundException($"Failure cause '{failureCauseId}' was not found.");
        }

        await EnsureFailureModeEditableAsync(failureCause.FailureModeId, cancellationToken);
        return failureCause;
    }

    private async Task EnsureRevisionEditableAsync(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        var revision = await _revisionRepository.GetByIdAsync(revisionId, cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException($"FMEA revision '{revisionId}' was not found.");
        }

        if (revision.Status is not FmeaRevisionStatus.Draft and not FmeaRevisionStatus.Rejected)
        {
            throw new ConflictException($"Revision '{revision.RevisionCode}' is not editable.");
        }
    }
}
