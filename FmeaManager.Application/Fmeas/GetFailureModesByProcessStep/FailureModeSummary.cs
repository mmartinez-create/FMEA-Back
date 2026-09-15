namespace FmeaManager.Application.Fmeas.GetFailureModesByProcessStep;

    public sealed record FailureModeSummary(
        Guid Id,
Guid ProcessStepId,
string Description);
