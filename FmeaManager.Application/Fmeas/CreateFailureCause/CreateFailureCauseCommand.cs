namespace FmeaManager.Application.Fmeas.CreateFailureCause;

public sealed record CreateFailureCauseCommand(
    Guid FailureModeId,
    string Description);
