namespace FmeaManager.Application.Fmeas.CreateFailureMode;

public sealed record CreateFailureModeCommand(
    Guid ProcessStepId,
    string Description);
