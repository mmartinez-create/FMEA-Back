namespace FmeaManager.Application.ProductStructure.CreateProductionLine;

public sealed record CreateProductionLineResult(
    Guid Id,
    Guid PlantId,
    string Code,
    string Name,
    bool IsActive);
