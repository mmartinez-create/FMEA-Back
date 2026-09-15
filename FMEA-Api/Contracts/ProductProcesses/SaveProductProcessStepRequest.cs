namespace FMEA_Api.Contracts.ProductProcesses;

public sealed record SaveProductProcessStepRequest(
    int Sequence,
    string Name,
    string? Function,
    string? Requirement);
