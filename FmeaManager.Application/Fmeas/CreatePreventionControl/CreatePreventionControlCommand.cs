namespace FmeaManager.Application.Fmeas.CreatePreventionControl;

public sealed record CreatePreventionControlCommand(
    Guid FailureCauseId,
    string Description);
