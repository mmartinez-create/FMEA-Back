using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Fmeas.GetSyncedProcessSteps;

public sealed class GetSyncedProcessStepsHandler
{
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IProductProcessRepository _productProcessRepository;
    private readonly IProductProcessStepRepository _sharedStepRepository;

    public GetSyncedProcessStepsHandler(
        IFmeaRevisionRepository revisionRepository,
        IFmeaRepository fmeaRepository,
        IProcessStepRepository processStepRepository,
        IProductProcessRepository productProcessRepository,
        IProductProcessStepRepository sharedStepRepository)
    {
        _revisionRepository = revisionRepository;
        _fmeaRepository = fmeaRepository;
        _processStepRepository = processStepRepository;
        _productProcessRepository = productProcessRepository;
        _sharedStepRepository = sharedStepRepository;
    }

    public async Task<IReadOnlyList<ProcessStepSyncSummary>> HandleAsync(
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

        var processSteps =
            await _processStepRepository.ListByRevisionIdAsync(
                revisionId,
                cancellationToken);

        if (processSteps.Count == 0)
        {
            return Array.Empty<ProcessStepSyncSummary>();
        }

        var fmea = await _fmeaRepository.GetByIdAsync(
            revision.FmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{revision.FmeaId}' was not found.");
        }

        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                fmea.ProjectId,
                cancellationToken);

        var sharedById = new Dictionary<Guid, ProductProcessStep>();

        if (sharedProcess is not null)
        {
            var sharedSteps =
                await _sharedStepRepository.ListByProductProcessIdAsync(
                    sharedProcess.Id,
                    cancellationToken);

            sharedById = sharedSteps.ToDictionary(
                step => step.Id);
        }

        var canReadLiveShared =
            revision.Status is FmeaRevisionStatus.Draft
                or FmeaRevisionStatus.Rejected;

        return processSteps
            .Select(step =>
            {
                ProductProcessStep? sharedStep = null;

                if (
                    canReadLiveShared &&
                    step.ProductProcessStepId.HasValue)
                {
                    sharedById.TryGetValue(
                        step.ProductProcessStepId.Value,
                        out sharedStep);
                }

                var isShared =
                    step.ProductProcessStepId.HasValue;

                var isLiveShared =
                    sharedStep is not null;

                return new ProcessStepSyncSummary(
                    step.Id,
                    step.FmeaRevisionId,
                    step.ProductProcessStepId,
                    isLiveShared
                        ? sharedStep!.Sequence
                        : step.Sequence,
                    isLiveShared
                        ? sharedStep!.Name
                        : step.Name,
                    isLiveShared
                        ? sharedStep!.Function
                        : step.Function,
                    isLiveShared
                        ? sharedStep!.Requirement
                        : step.Requirement,
                    isShared,
                    isLiveShared,
                    isLiveShared
                        ? "ASMF"
                        : isShared
                            ? "Snapshot"
                            : "Legacy");
            })
            .OrderBy(step => step.Sequence)
            .ToList();
    }
}
