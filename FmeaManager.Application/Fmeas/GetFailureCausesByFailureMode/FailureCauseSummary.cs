namespace FmeaManager.Application.Fmeas.GetFailureCausesByFailureMode;

    public sealed record FailureCauseSummary(
        Guid Id,
Guid FailureModeId,
string Description);
