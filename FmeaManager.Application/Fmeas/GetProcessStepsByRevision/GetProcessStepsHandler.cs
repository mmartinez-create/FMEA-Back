using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetProcessStepsByRevision;

public sealed class GetProcessStepsHandler
{
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IProcessStepRepository _processStepRepository;

    public GetProcessStepsHandler(
        IFmeaRevisionRepository revisionRepository,
        IProcessStepRepository processStepRepository)
    {
        _revisionRepository = revisionRepository;
        _processStepRepository = processStepRepository;
    }

    public async Task<IReadOnlyList<ProcessStepSummary>> HandleAsync(
        Guid revisionId,
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

        var processSteps = await _processStepRepository.ListByRevisionIdAsync(
            revisionId,
            cancellationToken);

        return processSteps
            .Select(step => new ProcessStepSummary(
                step.Id,
                step.FmeaRevisionId,
                step.Sequence,
                step.Name,
                step.Function,
                step.Requirement))
            .ToList();
    }
}
