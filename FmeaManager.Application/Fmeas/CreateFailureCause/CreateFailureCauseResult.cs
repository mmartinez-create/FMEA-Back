namespace FmeaManager.Application.Fmeas.CreateFailureCause;

public sealed record CreateFailureCauseResult(
    Guid Id,
    Guid FailureModeId,
    string Description);
