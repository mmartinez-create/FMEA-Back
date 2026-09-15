namespace FmeaManager.Application.Fmeas.SyncProductProcess;

public sealed record SyncProductProcessResult(
    bool HasSharedProcess,
    Guid? ProductProcessId,
    int SharedStepCount,
    int CreatedCount,
    int LinkedExistingCount,
    int RefreshedCount,
    int TotalPfmeaProcessSteps);
