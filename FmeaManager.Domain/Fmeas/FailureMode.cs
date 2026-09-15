namespace FmeaManager.Domain.Fmeas;

public sealed class FailureMode
{
    private FailureMode()
    {
    }

    private FailureMode(
        Guid id,
        Guid processStepId,
        string description)
    {
        Id = id;
        ProcessStepId = processStepId;
        Description = description;
    }

    public Guid Id { get; private set; }

    public Guid ProcessStepId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public static FailureMode Create(
        Guid processStepId,
        string description)
    {
        ValidateProcessStepId(processStepId);
        ValidateDescription(description);

        return new FailureMode(
            Guid.NewGuid(),
            processStepId,
            description.Trim());
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description.Trim();
    }

    private static void ValidateProcessStepId(Guid processStepId)
    {
        if (processStepId == Guid.Empty)
        {
            throw new ArgumentException(
                "Process step is required.",
                nameof(processStepId));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Failure mode description is required.",
                nameof(description));
        }
    }
}
