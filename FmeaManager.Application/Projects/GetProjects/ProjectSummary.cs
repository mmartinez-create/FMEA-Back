using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Projects.GetProjects;

public sealed record ProjectSummary(
    Guid Id,
    string Code,
    string Name,
    string? Product,
    string? Plant,
    Guid CustomerProfileId,
    string Owner,
    ProjectStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
