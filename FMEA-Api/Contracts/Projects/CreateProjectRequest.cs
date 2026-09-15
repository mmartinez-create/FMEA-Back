namespace FMEA_Api.Contracts.Projects;

public sealed record CreateProjectRequest(
    string Code,
    string Name,
    string? Product,
    string? Plant,
    Guid CustomerProfileId,
    string Owner);
