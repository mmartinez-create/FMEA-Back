using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.UpsertRiskAssessment;

public sealed class UpsertRiskAssessmentHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IRiskAssessmentRepository _riskAssessmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertRiskAssessmentHandler(
        FmeaEditabilityGuard editabilityGuard,
        IRiskAssessmentRepository riskAssessmentRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _riskAssessmentRepository = riskAssessmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RiskAssessmentResult> HandleAsync(
        UpsertRiskAssessmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var failureCause =
            await _editabilityGuard.EnsureFailureCauseEditableAsync(
                command.FailureCauseId,
                cancellationToken);

        var assessment =
            await _riskAssessmentRepository.GetByFailureCauseIdAsync(
                failureCause.Id,
                cancellationToken);

        if (assessment is null)
        {
            assessment = RiskAssessment.Create(
                failureCause.Id,
                command.Severity,
                command.Occurrence,
                command.Detection,
                command.User);

            _riskAssessmentRepository.Add(assessment);
        }
        else
        {
            assessment.UpdateRatings(
                command.Severity,
                command.Occurrence,
                command.Detection,
                command.User);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(assessment);
    }

    private static RiskAssessmentResult Map(RiskAssessment assessment)
    {
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
