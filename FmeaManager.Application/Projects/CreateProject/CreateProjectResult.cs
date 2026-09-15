using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Projects.CreateProject;

public sealed record CreateProjectResult(
    Guid Id,
    string Code,
    string Name,
    string? Product,
    string? Plant,
    Guid CustomerProfileId,
    string Owner,
    ProjectStatus Status,
    DateTime CreatedAt,
    string CreatedBy);
