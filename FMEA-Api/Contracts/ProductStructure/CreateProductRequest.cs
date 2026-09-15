namespace FMEA_Api.Contracts.ProductStructure;

public sealed record CreateProductRequest(
    string Code,
    string Name,
    string? PartNumber);
