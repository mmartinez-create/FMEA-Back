namespace FMEA_Api.Contracts.Fmeas;

public sealed record CreateProcessStepRequest(
    int Sequence,
    string Name,
    string? Function,
    string? Requirement);
