namespace FMEA_Api.Contracts.FailureAnalysis;

public sealed record UpsertRiskAssessmentRequest(
    int Severity,
    int Occurrence,
    int Detection);
