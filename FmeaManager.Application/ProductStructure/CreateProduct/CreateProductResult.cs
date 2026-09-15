namespace FmeaManager.Application.ProductStructure.CreateProduct;

public sealed record CreateProductResult(
    Guid Id,
    Guid ProductionLineId,
    string Code,
    string Name,
    string? PartNumber,
    bool IsActive);
