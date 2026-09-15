using FmeaManager.Domain.Fmeas;

namespace FMEA_Api.Contracts.Fmeas;

public sealed record CreateFmeaRequest(
    string Number,
    string Name,
    FmeaType Type,
    string Owner);
