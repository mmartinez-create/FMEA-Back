using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Fmeas.SyncProductProcess;

public sealed class PfmeaProcessSynchronizer
{
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IProductProcessRepository _productProcessRepository;
    private readonly IProductProcessStepRepository _sharedStepRepository;
    private readonly IPfmeaProcessSyncRepository _syncRepository;

    public PfmeaProcessSynchronizer(
        IFmeaRepository fmeaRepository,
        IProductProcessRepository productProcessRepository,
        IProductProcessStepRepository sharedStepRepository,
        IPfmeaProcessSyncRepository syncRepository)
    {
        _fmeaRepository = fmeaRepository;
        _productProcessRepository = productProcessRepository;
        _sharedStepRepository = sharedStepRepository;
        _syncRepository = syncRepository;
    }

    public async Task<SyncProductProcessResult> SyncAsync(
        FmeaRevision revision,
        CancellationToken cancellationToken = default)
    {
        if (
            revision.Status is not FmeaRevisionStatus.Draft
            and not FmeaRevisionStatus.Rejected)
        {
            throw new ConflictException(
                $"Revision '{revision.RevisionCode}' is not editable.");
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

        if (sharedProcess is null)
        {
            var existing =
                await _syncRepository.ListTrackedByRevisionIdAsync(
                    revision.Id,
                    cancellationToken);

            return new SyncProductProcessResult(
                false,
                null,
                0,
                0,
                0,
                0,
                existing.Count);
        }

        var sharedSteps =
            await _sharedStepRepository.ListByProductProcessIdAsync(
                sharedProcess.Id,
                cancellationToken);

        var pfmeaSteps =
            (await _syncRepository.ListTrackedByRevisionIdAsync(
                revision.Id,
                cancellationToken))
            .ToList();

        var createdCount = 0;
        var linkedExistingCount = 0;
        var refreshedCount = 0;

        foreach (var sharedStep in sharedSteps)
        {
            var linked = pfmeaSteps.FirstOrDefault(
                step =>
                    step.ProductProcessStepId == sharedStep.Id);

            if (linked is not null)
            {
                EnsureSequenceCanBeUsed(
                    pfmeaSteps,
                    linked,
                    sharedStep);

                linked.RefreshSnapshotFrom(sharedStep);
                refreshedCount++;
                continue;
            }

            var sameSequenceLegacy = pfmeaSteps.FirstOrDefault(
                step =>
                    !step.ProductProcessStepId.HasValue &&
                    step.Sequence == sharedStep.Sequence);

            if (sameSequenceLegacy is not null)
            {
                sameSequenceLegacy.LinkToProductProcessStep(
                    sharedStep);

                linkedExistingCount++;
                continue;
            }

            var sequenceCollision = pfmeaSteps.FirstOrDefault(
                step =>
                    step.Sequence == sharedStep.Sequence);

            if (sequenceCollision is not null)
            {
                throw new ConflictException(
                    $"ASMF sequence '{sharedStep.Sequence}' conflicts with an existing PFMEA Process Step.");
            }

            var created = ProcessStep.CreateLinked(
                revision.Id,
                sharedStep);

            _syncRepository.Add(created);
            pfmeaSteps.Add(created);
            createdCount++;
        }

        return new SyncProductProcessResult(
            true,
            sharedProcess.Id,
            sharedSteps.Count,
            createdCount,
            linkedExistingCount,
            refreshedCount,
            pfmeaSteps.Count);
    }

    private static void EnsureSequenceCanBeUsed(
        IReadOnlyCollection<ProcessStep> pfmeaSteps,
        ProcessStep linked,
        ProductProcessStep sharedStep)
    {
        var collision = pfmeaSteps.FirstOrDefault(
            step =>
                step.Id != linked.Id &&
                step.Sequence == sharedStep.Sequence);

        if (collision is not null)
        {
            throw new ConflictException(
                $"ASMF sequence '{sharedStep.Sequence}' conflicts with another PFMEA Process Step.");
        }
    }
}
