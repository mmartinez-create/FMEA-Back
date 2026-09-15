namespace FmeaManager.Application.Fmeas.GetSyncedProcessSteps;

public sealed record ProcessStepSyncSummary(
    Guid Id,
    Guid FmeaRevisionId,
    Guid? ProductProcessStepId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement,
    bool IsShared,
    bool IsLiveShared,
    string Source);
