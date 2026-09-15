namespace FmeaManager.Application.Fmeas.CreateRevision;

public sealed record CreateFmeaRevisionCommand(
    Guid FmeaId,
    Guid BasedOnRevisionId,
    string? RevisionReason,
    string CreatedBy);
