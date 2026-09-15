namespace FMEA_Api.Contracts.ProductStructure;

public sealed record CreateProjectForProductRequest(
    string Code,
    string Name,
    Guid CustomerProfileId,
    string Owner);
