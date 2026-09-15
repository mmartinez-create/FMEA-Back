namespace FmeaManager.Application.Fmeas.CreateProcessStep;

public sealed record CreateProcessStepResult(
    Guid Id,
    Guid FmeaRevisionId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement);
