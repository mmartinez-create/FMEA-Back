namespace FmeaManager.Application.ProductStructure.CreateProjectForProduct;

public sealed record CreateProjectForProductCommand(
    Guid ProductId,
    string Code,
    string Name,
    Guid CustomerProfileId,
    string Owner,
    string CreatedBy);
