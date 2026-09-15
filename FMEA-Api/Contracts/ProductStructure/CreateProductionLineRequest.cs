namespace FMEA_Api.Contracts.ProductStructure;

public sealed record CreateProductionLineRequest(
    string Code,
    string Name);
