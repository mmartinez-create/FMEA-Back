using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.DocumentTraceability;

public enum DocumentSynchronizationState
{
    Missing = 1,
    Ready = 2,
    NeedsSync = 3,
    Synchronized = 4,
    Frozen = 5,
    Attention = 6
}

public sealed record PfmeaDocumentTraceability(
    Guid FmeaId,
    string Number,
    string Name,
    Guid RevisionId,
    string RevisionCode,
    FmeaRevisionStatus RevisionStatus,
    int LinkedProcessStepCount,
    int TotalProcessStepCount);

public sealed record ProcessStepTraceabilityRow(
    Guid ProductProcessStepId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement,
    int PfmeaLinkedStepCount,
    int FailureModeCount,
    int FailureCauseCount,
    int ScoredCauseCount,
    int HighActionPriorityCount,
    int MediumActionPriorityCount,
    int LowActionPriorityCount,
    int ControlPlanCharacteristicCount,
    DocumentSynchronizationState PfmeaState,
    DocumentSynchronizationState ControlPlanState,
    DocumentSynchronizationState OverallState);

public sealed record ProjectDocumentTraceabilityResult(
    Guid ProjectId,
    Guid? ProductProcessId,
    string? ProductProcessName,
    int SharedProcessStepCount,
    DocumentSynchronizationState AsmfState,
    DocumentSynchronizationState PfmeaState,
    DocumentSynchronizationState ControlPlanState,
    DocumentSynchronizationState OverallState,
    IReadOnlyList<PfmeaDocumentTraceability> PfmeaDocuments,
    Guid? ControlPlanId,
    string? ControlPlanNumber,
    ControlPlanStatus? ControlPlanStatus,
    int ControlPlanCharacteristicCount,
    int FailureModeCount,
    int FailureCauseCount,
    int ScoredCauseCount,
    int HighActionPriorityCount,
    int MediumActionPriorityCount,
    int LowActionPriorityCount,
    IReadOnlyList<ProcessStepTraceabilityRow> ProcessSteps);

public sealed record SynchronizeProjectDocumentsResult(
    int PfmeaRevisionCount,
    int PfmeaCreatedLinks,
    int PfmeaLinkedLegacySteps,
    int PfmeaRefreshedSteps,
    int ControlPlanRefreshedItems,
    ProjectDocumentTraceabilityResult Traceability);
