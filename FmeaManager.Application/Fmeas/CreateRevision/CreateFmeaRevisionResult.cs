using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateRevision;

public sealed record CreateFmeaRevisionResult(
    Guid Id,
    Guid FmeaId,
    int RevisionNumber,
    string RevisionCode,
    Guid? BasedOnRevisionId,
    string? RevisionReason,
    FmeaRevisionStatus Status,
    DateTime CreatedAt,
    string CreatedBy);
