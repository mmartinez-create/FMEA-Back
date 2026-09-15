namespace FmeaManager.Domain.Fmeas;

public sealed class PreventionControl
{
    private PreventionControl()
    {
    }

    private PreventionControl(
        Guid id,
        Guid failureCauseId,
        string description)
    {
        Id = id;
        FailureCauseId = failureCauseId;
        Description = description;
    }

    public Guid Id { get; private set; }

    public Guid FailureCauseId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public static PreventionControl Create(
        Guid failureCauseId,
        string description)
    {
        ValidateFailureCauseId(failureCauseId);
        ValidateDescription(description);

        return new PreventionControl(
            Guid.NewGuid(),
            failureCauseId,
            description.Trim());
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description.Trim();
    }

    private static void ValidateFailureCauseId(Guid failureCauseId)
    {
        if (failureCauseId == Guid.Empty)
        {
            throw new ArgumentException(
                "Failure cause is required.",
                nameof(failureCauseId));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Prevention control description is required.",
                nameof(description));
        }
    }
}
