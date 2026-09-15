namespace FmeaManager.Application.Fmeas.GetProcessStepsByRevision;

public sealed record ProcessStepSummary(
    Guid Id,
    Guid FmeaRevisionId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement);
