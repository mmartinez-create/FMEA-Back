using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.UpsertRiskAssessment;

namespace FmeaManager.Application.Fmeas.GetRiskAssessment;

public sealed class GetRiskAssessmentHandler
{
    private readonly IFailureCauseRepository _failureCauseRepository;
    private readonly IRiskAssessmentRepository _riskAssessmentRepository;

    public GetRiskAssessmentHandler(
        IFailureCauseRepository failureCauseRepository,
        IRiskAssessmentRepository riskAssessmentRepository)
    {
        _failureCauseRepository = failureCauseRepository;
        _riskAssessmentRepository = riskAssessmentRepository;
    }

    public async Task<RiskAssessmentResult?> HandleAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        var failureCause =
            await _failureCauseRepository.GetByIdAsync(
                failureCauseId,
                cancellationToken);

        if (failureCause is null)
        {
            throw new NotFoundException(
                $"Failure cause '{failureCauseId}' was not found.");
        }

        var assessment =
            await _riskAssessmentRepository.GetByFailureCauseIdAsync(
                failureCauseId,
                cancellationToken);

        if (assessment is null)
        {
            return null;
        }

        return new RiskAssessmentResult(
            assessment.Id,
            assessment.FailureCauseId,
            assessment.Severity,
            assessment.Occurrence,
            assessment.Detection,
            assessment.Rpn,
            assessment.ActionPriority,
            assessment.CreatedAt,
            assessment.CreatedBy,
            assessment.UpdatedAt,
            assessment.UpdatedBy);
    }
}
