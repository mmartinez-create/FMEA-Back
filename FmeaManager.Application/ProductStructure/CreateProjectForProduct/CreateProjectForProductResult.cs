using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.ProductStructure.CreateProjectForProduct;

public sealed record CreateProjectForProductResult(
    Guid Id,
    Guid ProductId,
    string Code,
    string Name,
    string Product,
    string Plant,
    Guid CustomerProfileId,
    string Owner,
    ProjectStatus Status,
    DateTime CreatedAt);
