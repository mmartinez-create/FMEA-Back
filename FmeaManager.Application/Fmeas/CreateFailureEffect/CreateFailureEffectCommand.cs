namespace FmeaManager.Application.Fmeas.CreateFailureEffect;

public sealed record CreateFailureEffectCommand(
    Guid FailureModeId,
    string Description);
