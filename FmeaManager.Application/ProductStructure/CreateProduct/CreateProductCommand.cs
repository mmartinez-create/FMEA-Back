namespace FmeaManager.Application.ProductStructure.CreateProduct;

public sealed record CreateProductCommand(
    Guid ProductionLineId,
    string Code,
    string Name,
    string? PartNumber);
