using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.Fmeas;

public sealed class ProcessStep
{
    private ProcessStep()
    {
    }

    private ProcessStep(
        Guid id,
        Guid fmeaRevisionId,
        int sequence,
        string name,
        string? function,
        string? requirement)
    {
        Id = id;
        FmeaRevisionId = fmeaRevisionId;
        Sequence = sequence;
        Name = name;
        Function = function;
        Requirement = requirement;
    }

    public Guid Id { get; private set; }

    public Guid FmeaRevisionId { get; private set; }

    public Guid? ProductProcessStepId { get; private set; }

    public int Sequence { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Function { get; private set; }

    public string? Requirement { get; private set; }

    public static ProcessStep Create(
        Guid fmeaRevisionId,
        int sequence,
        string name,
        string? function = null,
        string? requirement = null)
    {
        ValidateRevisionId(fmeaRevisionId);
        ValidateSequence(sequence);
        ValidateName(name);

        return new ProcessStep(
            Guid.NewGuid(),
            fmeaRevisionId,
            sequence,
            name.Trim(),
            NormalizeOptional(function),
            NormalizeOptional(requirement));
    }

    public static ProcessStep CloneForRevision(
        Guid targetRevisionId,
        ProcessStep source)
    {
        ValidateRevisionId(targetRevisionId);
        ArgumentNullException.ThrowIfNull(source);

        var clone = new ProcessStep(
            Guid.NewGuid(),
            targetRevisionId,
            source.Sequence,
            source.Name,
            source.Function,
            source.Requirement);

        clone.ProductProcessStepId = source.ProductProcessStepId;
        return clone;
    }

    public static ProcessStep CreateLinked(
        Guid fmeaRevisionId,
        ProductProcessStep sharedStep)
    {
        ArgumentNullException.ThrowIfNull(sharedStep);
        ValidateRevisionId(fmeaRevisionId);

        var processStep = new ProcessStep(
            Guid.NewGuid(),
            fmeaRevisionId,
            sharedStep.Sequence,
            sharedStep.Name,
            sharedStep.Function,
            sharedStep.Requirement);

        processStep.ProductProcessStepId = sharedStep.Id;
        return processStep;
    }

    public void LinkToProductProcessStep(
        ProductProcessStep sharedStep)
    {
        ArgumentNullException.ThrowIfNull(sharedStep);

        ProductProcessStepId = sharedStep.Id;
        RefreshSnapshotFrom(sharedStep);
    }

    public void RefreshSnapshotFrom(
        ProductProcessStep sharedStep)
    {
        ArgumentNullException.ThrowIfNull(sharedStep);

        if (
            ProductProcessStepId.HasValue &&
            ProductProcessStepId.Value != sharedStep.Id)
        {
            throw new InvalidOperationException(
                "The PFMEA Process Step is linked to a different shared process step.");
        }

        ProductProcessStepId = sharedStep.Id;
        Sequence = sharedStep.Sequence;
        Name = sharedStep.Name;
        Function = sharedStep.Function;
        Requirement = sharedStep.Requirement;
    }

    public void UpdateInformation(
        int sequence,
        string name,
        string? function,
        string? requirement)
    {
        ValidateSequence(sequence);
        ValidateName(name);

        Sequence = sequence;
        Name = name.Trim();
        Function = NormalizeOptional(function);
        Requirement = NormalizeOptional(requirement);
    }

    private static void ValidateRevisionId(Guid fmeaRevisionId)
    {
        if (fmeaRevisionId == Guid.Empty)
        {
            throw new ArgumentException(
                "FMEA revision is required.",
                nameof(fmeaRevisionId));
        }
    }

    private static void ValidateSequence(int sequence)
    {
        if (sequence <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequence),
                sequence,
                "Process step sequence must be greater than zero.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Process step name is required.",
                nameof(name));
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
