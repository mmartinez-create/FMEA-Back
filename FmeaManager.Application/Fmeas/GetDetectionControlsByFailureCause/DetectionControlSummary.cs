namespace FmeaManager.Application.Fmeas.GetDetectionControlsByFailureCause;

    public sealed record DetectionControlSummary(
        Guid Id,
Guid FailureCauseId,
string Description);
