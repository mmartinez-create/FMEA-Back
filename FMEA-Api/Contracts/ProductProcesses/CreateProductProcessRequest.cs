namespace FMEA_Api.Contracts.ProductProcesses;

public sealed record CreateProductProcessRequest(
    string Name,
    string? Description);
