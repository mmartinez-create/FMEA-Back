using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.UpsertRiskAssessment;

public sealed record RiskAssessmentResult(
    Guid Id,
    Guid FailureCauseId,
    int Severity,
    int Occurrence,
    int Detection,
    int Rpn,
    ActionPriorityLevel ActionPriority,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy);
