namespace FMEA_Api.Contracts.Fmeas;

public sealed record CreateFmeaRevisionRequest(
    Guid BasedOnRevisionId,
    string? RevisionReason);
