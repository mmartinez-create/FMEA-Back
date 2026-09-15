namespace FmeaManager.Application.ProductProcesses.CreateStep;

public sealed record CreateProductProcessStepCommand(
    Guid ProductProcessId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement,
    string CreatedBy);
