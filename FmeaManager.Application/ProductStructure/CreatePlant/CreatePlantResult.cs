namespace FmeaManager.Application.ProductStructure.CreatePlant;

public sealed record CreatePlantResult(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);
