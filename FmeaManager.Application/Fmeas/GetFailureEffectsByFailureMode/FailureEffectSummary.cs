namespace FmeaManager.Application.Fmeas.GetFailureEffectsByFailureMode;

    public sealed record FailureEffectSummary(
        Guid Id,
Guid FailureModeId,
string Description);
