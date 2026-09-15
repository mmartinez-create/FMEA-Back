namespace FmeaManager.Application.Fmeas.CreatePreventionControl;

public sealed record CreatePreventionControlResult(
    Guid Id,
    Guid FailureCauseId,
    string Description);
