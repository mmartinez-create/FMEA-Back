namespace FMEA_Api.Contracts.ProductStructure;

public sealed record CreatePlantRequest(
    string Code,
    string Name);
