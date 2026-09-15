namespace FmeaManager.Application.ProductProcesses.UpdateStep;

public sealed record UpdateProductProcessStepCommand(
    Guid StepId,
    int Sequence,
    string Name,
    string? Function,
    string? Requirement,
    string UpdatedBy);
