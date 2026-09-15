namespace FmeaManager.Application.Fmeas.CreateDetectionControl;

public sealed record CreateDetectionControlCommand(
    Guid FailureCauseId,
    string Description);
