using FmeaManager.Domain.ControlPlans;

namespace FMEA_Api.Contracts.ControlPlans;

public sealed record SaveControlPlanItemRequest(
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
    string ReactionPlan);
