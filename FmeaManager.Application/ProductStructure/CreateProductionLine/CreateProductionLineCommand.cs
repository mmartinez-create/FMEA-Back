namespace FmeaManager.Application.ProductStructure.CreateProductionLine;

public sealed record CreateProductionLineCommand(
    Guid PlantId,
    string Code,
    string Name);
