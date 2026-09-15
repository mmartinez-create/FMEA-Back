using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.ProductStructure.GetProductStructure;

public sealed record ProductStructureTree(
    IReadOnlyList<PlantNode> Plants);

public sealed record ScopeAccessSummary(
    bool CanRead,
    bool CanEdit,
    bool CanApprove);

public sealed record PlantNode(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    ScopeAccessSummary Access,
    IReadOnlyList<ProductionLineNode> ProductionLines);

public sealed record ProductionLineNode(
    Guid Id,
    Guid PlantId,
    string Code,
    string Name,
    bool IsActive,
    ScopeAccessSummary Access,
    IReadOnlyList<ProductNode> Products);

public sealed record ProductNode(
    Guid Id,
    Guid ProductionLineId,
    string Code,
    string Name,
    string? PartNumber,
    bool IsActive,
    ScopeAccessSummary Access,
    IReadOnlyList<ProductProjectNode> Projects);

public sealed record ProductProjectNode(
    Guid Id,
    string Code,
    string Name,
    string Owner,
    ProjectStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    ScopeAccessSummary Access);
