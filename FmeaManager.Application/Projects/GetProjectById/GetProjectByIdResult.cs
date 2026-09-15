using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Projects.GetProjectById;

public sealed record GetProjectByIdResult(
    Guid Id,
    string Code,
    string Name,
    string? Product,
    string? Plant,
    Guid CustomerProfileId,
    string Owner,
    ProjectStatus Status,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
