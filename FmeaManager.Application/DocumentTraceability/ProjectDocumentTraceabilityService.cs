using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.SyncProductProcess;
using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.DocumentTraceability;

public sealed class ProjectDocumentTraceabilityService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProductProcessRepository _productProcessRepository;
    private readonly IProductProcessStepRepository _sharedStepRepository;
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IFailureModeRepository _failureModeRepository;
    private readonly IFailureCauseRepository _failureCauseRepository;
    private readonly IRiskAssessmentRepository _riskAssessmentRepository;
    private readonly IControlPlanRepository _controlPlanRepository;
    private readonly IControlPlanItemRepository _controlPlanItemRepository;
    private readonly PfmeaProcessSynchronizer _pfmeaSynchronizer;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectDocumentTraceabilityService(
        IProjectRepository projectRepository,
        IProductProcessRepository productProcessRepository,
        IProductProcessStepRepository sharedStepRepository,
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository,
        IProcessStepRepository processStepRepository,
        IFailureModeRepository failureModeRepository,
        IFailureCauseRepository failureCauseRepository,
        IRiskAssessmentRepository riskAssessmentRepository,
        IControlPlanRepository controlPlanRepository,
        IControlPlanItemRepository controlPlanItemRepository,
        PfmeaProcessSynchronizer pfmeaSynchronizer,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _productProcessRepository = productProcessRepository;
        _sharedStepRepository = sharedStepRepository;
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
        _processStepRepository = processStepRepository;
        _failureModeRepository = failureModeRepository;
        _failureCauseRepository = failureCauseRepository;
        _riskAssessmentRepository = riskAssessmentRepository;
        _controlPlanRepository = controlPlanRepository;
        _controlPlanItemRepository = controlPlanItemRepository;
        _pfmeaSynchronizer = pfmeaSynchronizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDocumentTraceabilityResult> GetAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        await EnsureProjectExistsAsync(
            projectId,
            cancellationToken);

        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                projectId,
                cancellationToken);

        if (sharedProcess is null)
        {
            return EmptyResult(projectId);
        }

        var sharedSteps =
            await _sharedStepRepository.ListByProductProcessIdAsync(
                sharedProcess.Id,
                cancellationToken);

        var sharedStepIds = sharedSteps
            .Select(step => step.Id)
            .ToHashSet();

        var pfmeaDocuments =
            new List<PfmeaDocumentTraceability>();

        var linkedStepsBySharedId =
            sharedSteps.ToDictionary(
                step => step.Id,
                _ => new List<ProcessStep>());

        var rowCounters =
            sharedSteps.ToDictionary(
                step => step.Id,
                _ => new RowCounter());

        var activeProcessFmeas =
            (await _fmeaRepository.ListByProjectIdAsync(
                projectId,
                cancellationToken))
            .Where(fmea =>
                fmea.IsActive &&
                fmea.Type is FmeaType.Pfmea
                    or FmeaType.FoundationPfmea)
            .ToList();

        foreach (var fmea in activeProcessFmeas)
        {
            var revisions =
                await _revisionRepository.ListByFmeaIdAsync(
                    fmea.Id,
                    cancellationToken);

            var latestRevision = revisions
                .OrderByDescending(revision =>
                    revision.RevisionNumber)
                .FirstOrDefault();

            if (latestRevision is null)
            {
                continue;
            }

            var pfmeaSteps =
                await _processStepRepository.ListByRevisionIdAsync(
                    latestRevision.Id,
                    cancellationToken);

            var linkedCount = 0;

            foreach (var pfmeaStep in pfmeaSteps)
            {
                if (
                    !pfmeaStep.ProductProcessStepId.HasValue ||
                    !sharedStepIds.Contains(
                        pfmeaStep.ProductProcessStepId.Value))
                {
                    continue;
                }

                var sharedStepId =
                    pfmeaStep.ProductProcessStepId.Value;

                linkedStepsBySharedId[sharedStepId]
                    .Add(pfmeaStep);

                linkedCount++;

                var modes =
                    await _failureModeRepository.ListByProcessStepIdAsync(
                        pfmeaStep.Id,
                        cancellationToken);

                rowCounters[sharedStepId].FailureModes +=
                    modes.Count;

                foreach (var mode in modes)
                {
                    var causes =
                        await _failureCauseRepository.ListByFailureModeIdAsync(
                            mode.Id,
                            cancellationToken);

                    rowCounters[sharedStepId].FailureCauses +=
                        causes.Count;

                    foreach (var cause in causes)
                    {
                        var risk =
                            await _riskAssessmentRepository.GetByFailureCauseIdAsync(
                                cause.Id,
                                cancellationToken);

                        if (risk is null)
                        {
                            continue;
                        }

                        var counter =
                            rowCounters[sharedStepId];

                        counter.ScoredCauses++;

                        switch (risk.ActionPriority)
                        {
                            case ActionPriorityLevel.High:
                                counter.High++;
                                break;

                            case ActionPriorityLevel.Medium:
                                counter.Medium++;
                                break;

                            case ActionPriorityLevel.Low:
                                counter.Low++;
                                break;
                        }
                    }
                }
            }

            pfmeaDocuments.Add(
                new PfmeaDocumentTraceability(
                    fmea.Id,
                    fmea.Number,
                    fmea.Name,
                    latestRevision.Id,
                    latestRevision.RevisionCode,
                    latestRevision.Status,
                    linkedCount,
                    pfmeaSteps.Count));
        }

        var controlPlan =
            await _controlPlanRepository.GetByProjectIdAsync(
                projectId,
                cancellationToken);

        IReadOnlyList<ControlPlanItem> controlPlanItems =
            Array.Empty<ControlPlanItem>();

        if (controlPlan is not null)
        {
            controlPlanItems =
                await _controlPlanItemRepository.ListByControlPlanIdAsync(
                    controlPlan.Id,
                    cancellationToken);

            foreach (var item in controlPlanItems)
            {
                if (rowCounters.TryGetValue(
                    item.ProductProcessStepId,
                    out var counter))
                {
                    counter.ControlPlanCharacteristics++;
                }
            }
        }

        var invalidControlPlanReferences =
            controlPlanItems.Any(item =>
                !sharedStepIds.Contains(
                    item.ProductProcessStepId));

        var distinctLinkedSharedSteps =
            linkedStepsBySharedId.Count(pair =>
                pair.Value.Count > 0);

        var asmfState =
            sharedSteps.Count == 0
                ? DocumentSynchronizationState.Missing
                : DocumentSynchronizationState.Ready;

        var pfmeaState =
            ProjectDocumentSyncStatusEvaluator.EvaluatePfmea(
                sharedSteps.Count,
                pfmeaDocuments.Count,
                distinctLinkedSharedSteps);

        var controlPlanState =
            ProjectDocumentSyncStatusEvaluator.EvaluateControlPlan(
                sharedSteps.Count,
                controlPlan is not null,
                controlPlanItems.Count,
                invalidControlPlanReferences);

        var overallState =
            ProjectDocumentSyncStatusEvaluator.EvaluateOverall(
                asmfState,
                pfmeaState,
                controlPlanState);

        var rows = sharedSteps
            .OrderBy(step => step.Sequence)
            .Select(step =>
            {
                var counter = rowCounters[step.Id];
                var pfmeaLinkedCount =
                    linkedStepsBySharedId[step.Id].Count;

                var rowPfmeaState =
                    pfmeaDocuments.Count == 0
                        ? DocumentSynchronizationState.Missing
                        : pfmeaLinkedCount > 0
                            ? DocumentSynchronizationState.Synchronized
                            : DocumentSynchronizationState.NeedsSync;

                var rowControlPlanState =
                    controlPlan is null
                        ? DocumentSynchronizationState.Missing
                        : counter.ControlPlanCharacteristics > 0
                            ? DocumentSynchronizationState.Synchronized
                            : DocumentSynchronizationState.Ready;

                var rowOverallState =
                    ProjectDocumentSyncStatusEvaluator.EvaluateOverall(
                        DocumentSynchronizationState.Ready,
                        rowPfmeaState,
                        rowControlPlanState);

                return new ProcessStepTraceabilityRow(
                    step.Id,
                    step.Sequence,
                    step.Name,
                    step.Function,
                    step.Requirement,
                    pfmeaLinkedCount,
                    counter.FailureModes,
                    counter.FailureCauses,
                    counter.ScoredCauses,
                    counter.High,
                    counter.Medium,
                    counter.Low,
                    counter.ControlPlanCharacteristics,
                    rowPfmeaState,
                    rowControlPlanState,
                    rowOverallState);
            })
            .ToList();

        return new ProjectDocumentTraceabilityResult(
            projectId,
            sharedProcess.Id,
            sharedProcess.Name,
            sharedSteps.Count,
            asmfState,
            pfmeaState,
            controlPlanState,
            overallState,
            pfmeaDocuments,
            controlPlan?.Id,
            controlPlan?.Number,
            controlPlan?.Status,
            controlPlanItems.Count,
            rows.Sum(row => row.FailureModeCount),
            rows.Sum(row => row.FailureCauseCount),
            rows.Sum(row => row.ScoredCauseCount),
            rows.Sum(row => row.HighActionPriorityCount),
            rows.Sum(row => row.MediumActionPriorityCount),
            rows.Sum(row => row.LowActionPriorityCount),
            rows);
    }

    public async Task<SynchronizeProjectDocumentsResult> SynchronizeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        await EnsureProjectExistsAsync(
            projectId,
            cancellationToken);

        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                projectId,
                cancellationToken);

        if (sharedProcess is null)
        {
            throw new ConflictException(
                "Create the ASMF / Product Process before synchronizing the project document chain.");
        }

        var sharedSteps =
            await _sharedStepRepository.ListByProductProcessIdAsync(
                sharedProcess.Id,
                cancellationToken);

        if (sharedSteps.Count == 0)
        {
            throw new ConflictException(
                "Add at least one ASMF Process Step before synchronizing the project document chain.");
        }

        var pfmeaRevisionCount = 0;
        var createdLinks = 0;
        var linkedLegacySteps = 0;
        var refreshedSteps = 0;

        var fmeas =
            (await _fmeaRepository.ListByProjectIdAsync(
                projectId,
                cancellationToken))
            .Where(fmea =>
                fmea.IsActive &&
                fmea.Type is FmeaType.Pfmea
                    or FmeaType.FoundationPfmea)
            .ToList();

        foreach (var fmea in fmeas)
        {
            var revisions =
                await _revisionRepository.ListByFmeaIdAsync(
                    fmea.Id,
                    cancellationToken);

            var latest = revisions
                .OrderByDescending(revision =>
                    revision.RevisionNumber)
                .FirstOrDefault();

            if (
                latest is null ||
                latest.Status is not FmeaRevisionStatus.Draft
                    and not FmeaRevisionStatus.Rejected)
            {
                continue;
            }

            var trackedRevision =
                await _revisionRepository.GetByIdAsync(
                    latest.Id,
                    cancellationToken);

            if (trackedRevision is null)
            {
                continue;
            }

            var result =
                await _pfmeaSynchronizer.SyncAsync(
                    trackedRevision,
                    cancellationToken);

            pfmeaRevisionCount++;
            createdLinks += result.CreatedCount;
            linkedLegacySteps += result.LinkedExistingCount;
            refreshedSteps += result.RefreshedCount;
        }

        var refreshedControlPlanItems = 0;

        var controlPlan =
            await _controlPlanRepository.GetByProjectIdAsync(
                projectId,
                cancellationToken);

        if (
            controlPlan is not null &&
            controlPlan.IsEditable)
        {
            var items =
                await _controlPlanItemRepository.ListByControlPlanIdAsync(
                    controlPlan.Id,
                    cancellationToken);

            var sharedById =
                sharedSteps.ToDictionary(step => step.Id);

            foreach (var item in items)
            {
                if (
                    sharedById.TryGetValue(
                        item.ProductProcessStepId,
                        out var sharedStep))
                {
                    item.RefreshProcessSnapshot(sharedStep);
                    refreshedControlPlanItems++;
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        var traceability =
            await GetAsync(
                projectId,
                cancellationToken);

        return new SynchronizeProjectDocumentsResult(
            pfmeaRevisionCount,
            createdLinks,
            linkedLegacySteps,
            refreshedSteps,
            refreshedControlPlanItems,
            traceability);
    }

    private async Task EnsureProjectExistsAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project =
            await _projectRepository.GetByIdAsync(
                projectId,
                cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{projectId}' was not found.");
        }
    }

    private static ProjectDocumentTraceabilityResult EmptyResult(
        Guid projectId)
    {
        return new ProjectDocumentTraceabilityResult(
            projectId,
            null,
            null,
            0,
            DocumentSynchronizationState.Missing,
            DocumentSynchronizationState.Missing,
            DocumentSynchronizationState.Missing,
            DocumentSynchronizationState.Missing,
            Array.Empty<PfmeaDocumentTraceability>(),
            null,
            null,
            null,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            Array.Empty<ProcessStepTraceabilityRow>());
    }

    private sealed class RowCounter
    {
        public int FailureModes { get; set; }
        public int FailureCauses { get; set; }
        public int ScoredCauses { get; set; }
        public int High { get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
        public int ControlPlanCharacteristics { get; set; }
    }
}
