namespace FmeaManager.Application.Fmeas.CreateProcessStep;

public sealed record CreateProcessStepCommand(
    Guid FmeaRevisionId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement);
