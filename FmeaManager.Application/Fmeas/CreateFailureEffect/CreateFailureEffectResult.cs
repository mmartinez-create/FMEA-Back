namespace FmeaManager.Application.Fmeas.CreateFailureEffect;

public sealed record CreateFailureEffectResult(
    Guid Id,
    Guid FailureModeId,
    string Description);
