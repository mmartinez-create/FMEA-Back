namespace FmeaManager.Domain.Fmeas;

public sealed class RiskAssessment
{
    private RiskAssessment()
    {
    }

    private RiskAssessment(
        Guid id,
        Guid failureCauseId,
        int severity,
        int occurrence,
        int detection,
        DateTime createdAt,
        string createdBy)
    {
        Id = id;
        FailureCauseId = failureCauseId;
        SetRatings(severity, occurrence, detection);
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    public Guid Id { get; private set; }

    public Guid FailureCauseId { get; private set; }

    public int Severity { get; private set; }

    public int Occurrence { get; private set; }

    public int Detection { get; private set; }

    public int Rpn { get; private set; }

    public ActionPriorityLevel ActionPriority =>
        ActionPriorityCalculator.Calculate(
            Severity,
            Occurrence,
            Detection);

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }

    public static RiskAssessment Create(
        Guid failureCauseId,
        int severity,
        int occurrence,
        int detection,
        string createdBy)
    {
        ValidateFailureCauseId(failureCauseId);
        ValidateUser(createdBy, nameof(createdBy));

        return new RiskAssessment(
            Guid.NewGuid(),
            failureCauseId,
            severity,
            occurrence,
            detection,
            DateTime.UtcNow,
            createdBy.Trim());
    }

    public void UpdateRatings(
        int severity,
        int occurrence,
        int detection,
        string updatedBy)
    {
        ValidateUser(updatedBy, nameof(updatedBy));
        SetRatings(severity, occurrence, detection);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy.Trim();
    }

    private void SetRatings(
        int severity,
        int occurrence,
        int detection)
    {
        ValidateRating(severity, nameof(severity));
        ValidateRating(occurrence, nameof(occurrence));
        ValidateRating(detection, nameof(detection));

        Severity = severity;
        Occurrence = occurrence;
        Detection = detection;
        Rpn = severity * occurrence * detection;
    }

    private static void ValidateRating(
        int value,
        string parameterName)
    {
        if (value is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "FMEA ratings must be between 1 and 10.");
        }
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

    private static void ValidateUser(
        string user,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            throw new ArgumentException(
                "User is required.",
                parameterName);
        }
    }
}
