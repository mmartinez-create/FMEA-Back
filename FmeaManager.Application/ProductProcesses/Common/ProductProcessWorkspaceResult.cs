namespace FmeaManager.Application.ProductProcesses.Common;

public sealed record ProductProcessWorkspaceResult(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    IReadOnlyList<ProductProcessStepResult> Steps);

public sealed record ProductProcessStepResult(
    Guid Id,
    Guid ProductProcessId,
    Guid ProjectId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy);
