namespace FmeaManager.Application.Fmeas.UpsertRiskAssessment;

public sealed record UpsertRiskAssessmentCommand(
    Guid FailureCauseId,
    int Severity,
    int Occurrence,
    int Detection,
    string User);
