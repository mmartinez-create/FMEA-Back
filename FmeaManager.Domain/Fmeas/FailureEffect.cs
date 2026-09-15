namespace FmeaManager.Domain.Fmeas;

public sealed class FailureEffect
{
    private FailureEffect()
    {
    }

    private FailureEffect(
        Guid id,
        Guid failureModeId,
        string description)
    {
        Id = id;
        FailureModeId = failureModeId;
        Description = description;
    }

    public Guid Id { get; private set; }

    public Guid FailureModeId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public static FailureEffect Create(
        Guid failureModeId,
        string description)
    {
        ValidateFailureModeId(failureModeId);
        ValidateDescription(description);

        return new FailureEffect(
            Guid.NewGuid(),
            failureModeId,
            description.Trim());
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description.Trim();
    }

    private static void ValidateFailureModeId(Guid failureModeId)
    {
        if (failureModeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Failure mode is required.",
                nameof(failureModeId));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Failure effect description is required.",
                nameof(description));
        }
    }
}
