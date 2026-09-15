namespace FmeaManager.Application.Fmeas.CreateFailureMode;

public sealed record CreateFailureModeResult(
    Guid Id,
    Guid ProcessStepId,
    string Description);
