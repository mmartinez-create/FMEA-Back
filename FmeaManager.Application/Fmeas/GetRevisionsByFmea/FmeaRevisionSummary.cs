using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.GetRevisionsByFmea;

public sealed record FmeaRevisionSummary(
    Guid Id,
    Guid FmeaId,
    int RevisionNumber,
    string RevisionCode,
    Guid? BasedOnRevisionId,
    string? RevisionReason,
    FmeaRevisionStatus Status,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    string? ApprovedBy,
    DateTime? RejectedAt,
    string? RejectionReason);
