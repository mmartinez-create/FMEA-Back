using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.ControlPlans;

public sealed class ControlPlanItem
{
    private ControlPlanItem() { }

    private ControlPlanItem(
        Guid id, Guid controlPlanId, Guid productProcessStepId,
        int processStepSequenceSnapshot, string processStepNameSnapshot,
        string? characteristicNumber, ControlPlanCharacteristicType characteristicType,
        string characteristicName, string? machineTooling, string? specialCharacteristic,
        string specificationTolerance, string evaluationMeasurementTechnique,
        int? sampleSize, string sampleFrequency, string controlMethod,
        string reactionPlan, string createdBy, DateTime createdAt)
    {
        Id = id;
        ControlPlanId = controlPlanId;
        ProductProcessStepId = productProcessStepId;
        ProcessStepSequenceSnapshot = processStepSequenceSnapshot;
        ProcessStepNameSnapshot = processStepNameSnapshot;
        CharacteristicNumber = characteristicNumber;
        CharacteristicType = characteristicType;
        CharacteristicName = characteristicName;
        MachineTooling = machineTooling;
        SpecialCharacteristic = specialCharacteristic;
        SpecificationTolerance = specificationTolerance;
        EvaluationMeasurementTechnique = evaluationMeasurementTechnique;
        SampleSize = sampleSize;
        SampleFrequency = sampleFrequency;
        ControlMethod = controlMethod;
        ReactionPlan = reactionPlan;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid ControlPlanId { get; private set; }
    public Guid ProductProcessStepId { get; private set; }
    public int ProcessStepSequenceSnapshot { get; private set; }
    public string ProcessStepNameSnapshot { get; private set; } = string.Empty;
    public string? CharacteristicNumber { get; private set; }
    public ControlPlanCharacteristicType CharacteristicType { get; private set; }
    public string CharacteristicName { get; private set; } = string.Empty;
    public string? MachineTooling { get; private set; }
    public string? SpecialCharacteristic { get; private set; }
    public string SpecificationTolerance { get; private set; } = string.Empty;
    public string EvaluationMeasurementTechnique { get; private set; } = string.Empty;
    public int? SampleSize { get; private set; }
    public string SampleFrequency { get; private set; } = string.Empty;
    public string ControlMethod { get; private set; } = string.Empty;
    public string ReactionPlan { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    public static ControlPlanItem Create(
        Guid controlPlanId,
        ProductProcessStep sharedStep,
        string? characteristicNumber,
        ControlPlanCharacteristicType characteristicType,
        string characteristicName,
        string? machineTooling,
        string? specialCharacteristic,
        string specificationTolerance,
        string evaluationMeasurementTechnique,
        int? sampleSize,
        string sampleFrequency,
        string controlMethod,
        string reactionPlan,
        string createdBy)
    {
        if (controlPlanId == Guid.Empty) throw new ArgumentException("Control Plan is required.", nameof(controlPlanId));
        ArgumentNullException.ThrowIfNull(sharedStep);

        Validate(characteristicType, characteristicName, specificationTolerance,
            evaluationMeasurementTechnique, sampleSize, sampleFrequency,
            controlMethod, reactionPlan, createdBy);

        return new ControlPlanItem(
            Guid.NewGuid(),
            controlPlanId,
            sharedStep.Id,
            sharedStep.Sequence,
            sharedStep.Name,
            NormalizeOptional(characteristicNumber),
            characteristicType,
            characteristicName.Trim(),
            NormalizeOptional(machineTooling),
            NormalizeOptional(specialCharacteristic),
            specificationTolerance.Trim(),
            evaluationMeasurementTechnique.Trim(),
            sampleSize,
            sampleFrequency.Trim(),
            controlMethod.Trim(),
            reactionPlan.Trim(),
            createdBy.Trim().ToLowerInvariant(),
            DateTime.UtcNow);
    }

    public void Update(
        ProductProcessStep sharedStep,
        string? characteristicNumber,
        ControlPlanCharacteristicType characteristicType,
        string characteristicName,
        string? machineTooling,
        string? specialCharacteristic,
        string specificationTolerance,
        string evaluationMeasurementTechnique,
        int? sampleSize,
        string sampleFrequency,
        string controlMethod,
        string reactionPlan,
        string updatedBy)
    {
        ArgumentNullException.ThrowIfNull(sharedStep);

        Validate(characteristicType, characteristicName, specificationTolerance,
            evaluationMeasurementTechnique, sampleSize, sampleFrequency,
            controlMethod, reactionPlan, updatedBy);

        ProductProcessStepId = sharedStep.Id;
        RefreshProcessSnapshot(sharedStep);
        CharacteristicNumber = NormalizeOptional(characteristicNumber);
        CharacteristicType = characteristicType;
        CharacteristicName = characteristicName.Trim();
        MachineTooling = NormalizeOptional(machineTooling);
        SpecialCharacteristic = NormalizeOptional(specialCharacteristic);
        SpecificationTolerance = specificationTolerance.Trim();
        EvaluationMeasurementTechnique = evaluationMeasurementTechnique.Trim();
        SampleSize = sampleSize;
        SampleFrequency = sampleFrequency.Trim();
        ControlMethod = controlMethod.Trim();
        ReactionPlan = reactionPlan.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy.Trim().ToLowerInvariant();
    }

    public void RefreshProcessSnapshot(ProductProcessStep sharedStep)
    {
        ArgumentNullException.ThrowIfNull(sharedStep);

        if (sharedStep.Id != ProductProcessStepId)
            throw new InvalidOperationException(
                "The Control Plan item belongs to a different Product Process Step.");

        ProcessStepSequenceSnapshot = sharedStep.Sequence;
        ProcessStepNameSnapshot = sharedStep.Name;
    }

    private static void Validate(
        ControlPlanCharacteristicType characteristicType,
        string characteristicName,
        string specificationTolerance,
        string evaluationMeasurementTechnique,
        int? sampleSize,
        string sampleFrequency,
        string controlMethod,
        string reactionPlan,
        string user)
    {
        if (!Enum.IsDefined(characteristicType))
            throw new ArgumentOutOfRangeException(nameof(characteristicType));

        Required(characteristicName, nameof(characteristicName), "Characteristic name");
        Required(specificationTolerance, nameof(specificationTolerance), "Specification / tolerance");
        Required(evaluationMeasurementTechnique, nameof(evaluationMeasurementTechnique), "Evaluation / measurement technique");
        if (sampleSize.HasValue && sampleSize.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(sampleSize), "Sample size must be greater than zero.");
        Required(sampleFrequency, nameof(sampleFrequency), "Sample frequency");
        Required(controlMethod, nameof(controlMethod), "Control method");
        Required(reactionPlan, nameof(reactionPlan), "Reaction plan");
        Required(user, nameof(user), "User");
    }

    private static void Required(string value, string parameterName, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{displayName} is required.", parameterName);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
