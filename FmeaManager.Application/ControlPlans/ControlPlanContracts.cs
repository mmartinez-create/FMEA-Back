using FmeaManager.Domain.ControlPlans;

namespace FmeaManager.Application.ControlPlans;

public sealed record CreateControlPlanCommand(
    Guid ProjectId,
    string Number,
    string Name,
    string CreatedBy);

public sealed record SaveControlPlanItemCommand(
    Guid ProjectId,
    Guid ProductProcessStepId,
    string? CharacteristicNumber,
    ControlPlanCharacteristicType CharacteristicType,
    string CharacteristicName,
    string? MachineTooling,
    string? SpecialCharacteristic,
    string SpecificationTolerance,
    string EvaluationMeasurementTechnique,
    int? SampleSize,
    string SampleFrequency,
    string ControlMethod,
    string ReactionPlan,
    string UserKey);

public sealed record RejectControlPlanCommand(
    Guid ProjectId,
    string Reason,
    string UserKey);

public sealed record ControlPlanItemResult(
    Guid Id,
    Guid ControlPlanId,
    Guid ProductProcessStepId,
    int ProcessStepSequence,
    string ProcessStepName,
    string? CharacteristicNumber,
    ControlPlanCharacteristicType CharacteristicType,
    string CharacteristicName,
    string? MachineTooling,
    string? SpecialCharacteristic,
    string SpecificationTolerance,
    string EvaluationMeasurementTechnique,
    int? SampleSize,
    string SampleFrequency,
    string ControlMethod,
    string ReactionPlan,
    bool IsLiveShared,
    string ProcessSource,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy);

public sealed record ControlPlanWorkspaceResult(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Name,
    ControlPlanStatus Status,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    string? ApprovedBy,
    DateTime? RejectedAt,
    string? RejectedBy,
    string? RejectionReason,
    bool IsEditable,
    IReadOnlyList<ControlPlanItemResult> Items);

public sealed record ControlPlanWorkflowResult(
    Guid Id,
    Guid ProjectId,
    ControlPlanStatus Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    string? ApprovedBy,
    DateTime? RejectedAt,
    string? RejectedBy,
    string? RejectionReason);
