using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.ChangeRevisionStatus;

public sealed record RevisionStatusResult(
    Guid Id,
    Guid FmeaId,
    int RevisionNumber,
    string RevisionCode,
    FmeaRevisionStatus Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    string? ApprovedBy,
    DateTime? RejectedAt,
    string? RejectionReason);
