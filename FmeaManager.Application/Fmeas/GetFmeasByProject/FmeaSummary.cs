using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.GetFmeasByProject;

public sealed record FmeaSummary(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Name,
    FmeaType Type,
    string Owner,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
