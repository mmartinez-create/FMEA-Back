namespace FmeaManager.Application.ProductProcesses.Create;

public sealed record CreateProductProcessCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    string CreatedBy);
