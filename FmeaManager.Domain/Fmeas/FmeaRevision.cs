namespace FmeaManager.Domain.Fmeas;

public sealed class FmeaRevision
{
    private FmeaRevision()
    {
    }

    private FmeaRevision(
        Guid id,
        Guid fmeaId,
        int revisionNumber,
        Guid? basedOnRevisionId,
        string? revisionReason,
        DateTime createdAt,
        string createdBy)
    {
        Id = id;
        FmeaId = fmeaId;
        RevisionNumber = revisionNumber;
        BasedOnRevisionId = basedOnRevisionId;
        RevisionReason = NormalizeOptional(revisionReason);
        Status = FmeaRevisionStatus.Draft;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    public Guid Id { get; private set; }

    public Guid FmeaId { get; private set; }

    public int RevisionNumber { get; private set; }

    public string RevisionCode => RevisionNumber.ToString("00");

    public Guid? BasedOnRevisionId { get; private set; }

    public string? RevisionReason { get; private set; }

    public FmeaRevisionStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? SubmittedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public string? ApprovedBy { get; private set; }

    public DateTime? RejectedAt { get; private set; }

    public string? RejectionReason { get; private set; }

    public static FmeaRevision CreateInitial(Guid fmeaId, string createdBy)
    {
        ValidateFmeaId(fmeaId);
        ValidateUser(createdBy, nameof(createdBy));

        return new FmeaRevision(
            Guid.NewGuid(),
            fmeaId,
            revisionNumber: 1,
            basedOnRevisionId: null,
            revisionReason: "Initial revision",
            DateTime.UtcNow,
            createdBy.Trim());
    }

    public static FmeaRevision CreateNextFrom(
        FmeaRevision previousRevision,
        string createdBy,
        string? revisionReason)
    {
        ArgumentNullException.ThrowIfNull(previousRevision);
        ValidateUser(createdBy, nameof(createdBy));

        if (previousRevision.Status != FmeaRevisionStatus.Approved)
        {
            throw new InvalidOperationException(
                "A new revision can only be created from an approved revision.");
        }

        return new FmeaRevision(
            Guid.NewGuid(),
            previousRevision.FmeaId,
            previousRevision.RevisionNumber + 1,
            previousRevision.Id,
            revisionReason,
            DateTime.UtcNow,
            createdBy.Trim());
    }

    public void UpdateRevisionReason(string? revisionReason)
    {
        EnsureEditable();
        RevisionReason = NormalizeOptional(revisionReason);
    }

    public void SubmitForReview()
    {
        EnsureStatus(
            FmeaRevisionStatus.Draft,
            "Only a draft revision can be submitted for review.");

        Status = FmeaRevisionStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        RejectionReason = null;
        RejectedAt = null;
    }

    public void Approve(string approvedBy)
    {
        ValidateUser(approvedBy, nameof(approvedBy));
        EnsureStatus(
            FmeaRevisionStatus.UnderReview,
            "Only a revision under review can be approved.");

        Status = FmeaRevisionStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy.Trim();
    }

    public void Reject(string reason)
    {
        EnsureStatus(
            FmeaRevisionStatus.UnderReview,
            "Only a revision under review can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "Rejection reason is required.",
                nameof(reason));
        }

        Status = FmeaRevisionStatus.Rejected;
        RejectionReason = reason.Trim();
        RejectedAt = DateTime.UtcNow;
    }

    public void ReturnToDraft()
    {
        EnsureStatus(
            FmeaRevisionStatus.Rejected,
            "Only a rejected revision can return to draft.");

        Status = FmeaRevisionStatus.Draft;
    }

    public void Supersede()
    {
        EnsureStatus(
            FmeaRevisionStatus.Approved,
            "Only an approved revision can be superseded.");

        Status = FmeaRevisionStatus.Superseded;
    }

    private void EnsureEditable()
    {
        if (Status is not FmeaRevisionStatus.Draft and not FmeaRevisionStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Only draft or rejected revisions can be modified.");
        }
    }

    private void EnsureStatus(FmeaRevisionStatus requiredStatus, string message)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void ValidateFmeaId(Guid fmeaId)
    {
        if (fmeaId == Guid.Empty)
        {
            throw new ArgumentException(
                "FMEA is required.",
                nameof(fmeaId));
        }
    }

    private static void ValidateUser(string user, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            throw new ArgumentException(
                "User is required.",
                parameterName);
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
