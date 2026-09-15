using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateFmea;

public sealed record CreateFmeaCommand(
    Guid ProjectId,
    string Number,
    string Name,
    FmeaType Type,
    string Owner,
    string CreatedBy);
