namespace FmeaManager.Application.Fmeas.GetPreventionControlsByFailureCause;

    public sealed record PreventionControlSummary(
        Guid Id,
Guid FailureCauseId,
string Description);
