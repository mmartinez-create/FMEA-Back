namespace FmeaManager.Domain.ControlPlans;

public sealed class ControlPlan
{
    private ControlPlan() { }

    private ControlPlan(Guid id, Guid projectId, string number, string name, string createdBy, DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Number = number;
        Name = name;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        Status = ControlPlanStatus.Draft;
    }

    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ControlPlanStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? RejectedBy { get; private set; }
    public string? RejectionReason { get; private set; }

    public bool IsEditable => Status is ControlPlanStatus.Draft or ControlPlanStatus.Rejected;

    public static ControlPlan Create(Guid projectId, string number, string name, string createdBy)
    {
        if (projectId == Guid.Empty) throw new ArgumentException("Project is required.", nameof(projectId));
        ValidateRequired(number, nameof(number), "Control Plan number");
        ValidateRequired(name, nameof(name), "Control Plan name");
        ValidateRequired(createdBy, nameof(createdBy), "User");

        return new ControlPlan(
            Guid.NewGuid(),
            projectId,
            number.Trim().ToUpperInvariant(),
            name.Trim(),
            createdBy.Trim().ToLowerInvariant(),
            DateTime.UtcNow);
    }

    public void EnsureEditable()
    {
        if (!IsEditable)
            throw new InvalidOperationException("Only Draft or Rejected Control Plans can be modified.");
    }

    public void SubmitForReview()
    {
        EnsureEditable();
        Status = ControlPlanStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        RejectedAt = null;
        RejectedBy = null;
        RejectionReason = null;
    }

    public void Approve(string approvedBy)
    {
        ValidateRequired(approvedBy, nameof(approvedBy), "Approver");
        if (Status != ControlPlanStatus.UnderReview)
            throw new InvalidOperationException("Only a Control Plan under review can be approved.");

        Status = ControlPlanStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy.Trim().ToLowerInvariant();
    }

    public void Reject(string reason, string rejectedBy)
    {
        ValidateRequired(reason, nameof(reason), "Rejection reason");
        ValidateRequired(rejectedBy, nameof(rejectedBy), "Reviewer");
        if (Status != ControlPlanStatus.UnderReview)
            throw new InvalidOperationException("Only a Control Plan under review can be rejected.");

        Status = ControlPlanStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
        RejectedBy = rejectedBy.Trim().ToLowerInvariant();
        RejectionReason = reason.Trim();
    }

    private static void ValidateRequired(string value, string parameterName, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{displayName} is required.", parameterName);
    }
}
