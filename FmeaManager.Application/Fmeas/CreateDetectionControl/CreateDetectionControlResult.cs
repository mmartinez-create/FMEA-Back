namespace FmeaManager.Application.Fmeas.CreateDetectionControl;

public sealed record CreateDetectionControlResult(
    Guid Id,
    Guid FailureCauseId,
    string Description);
