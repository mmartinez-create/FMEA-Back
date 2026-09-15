using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.GetFmeaById;

public sealed record GetFmeaByIdResult(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Name,
    FmeaType Type,
    string Owner,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
