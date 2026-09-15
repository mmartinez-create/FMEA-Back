using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IRiskAssessmentRepository
{
    Task<RiskAssessment?> GetByFailureCauseIdAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default);

    void Add(RiskAssessment riskAssessment);
}
