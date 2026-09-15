namespace FmeaManager.Application.Projects.CreateProject;

public sealed record CreateProjectCommand(
    string Code,
    string Name,
    string? Product,
    string? Plant,
    Guid CustomerProfileId,
    string Owner,
    string CreatedBy);
