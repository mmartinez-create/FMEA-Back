using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateFmea;

public sealed record CreateFmeaResult(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Name,
    FmeaType Type,
    string Owner,
    Guid InitialRevisionId,
    string InitialRevisionCode,
    FmeaRevisionStatus InitialRevisionStatus,
    DateTime CreatedAt,
    string CreatedBy);
