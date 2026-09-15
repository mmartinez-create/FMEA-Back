using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Revisioning;

public sealed class FmeaRevisionContentCloner : IFmeaRevisionContentCloner
{
    private readonly FmeaManagerDbContext _dbContext;

    public FmeaRevisionContentCloner(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CloneAsync(
        Guid sourceRevisionId,
        Guid targetRevisionId,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var sourceSteps = await _dbContext.ProcessSteps
            .AsNoTracking()
            .Where(step => step.FmeaRevisionId == sourceRevisionId)
            .OrderBy(step => step.Sequence)
            .ToListAsync(cancellationToken);

        if (sourceSteps.Count == 0)
        {
            return;
        }

        var sourceStepIds = sourceSteps.Select(step => step.Id).ToArray();

        var sourceModes = await _dbContext.FailureModes
            .AsNoTracking()
            .Where(mode => sourceStepIds.Contains(mode.ProcessStepId))
            .ToListAsync(cancellationToken);

        var sourceModeIds = sourceModes.Select(mode => mode.Id).ToArray();

        var sourceEffects = sourceModeIds.Length == 0
            ? new List<FailureEffect>()
            : await _dbContext.FailureEffects
                .AsNoTracking()
                .Where(effect => sourceModeIds.Contains(effect.FailureModeId))
                .ToListAsync(cancellationToken);

        var sourceCauses = sourceModeIds.Length == 0
            ? new List<FailureCause>()
            : await _dbContext.FailureCauses
                .AsNoTracking()
                .Where(cause => sourceModeIds.Contains(cause.FailureModeId))
                .ToListAsync(cancellationToken);

        var sourceCauseIds = sourceCauses.Select(cause => cause.Id).ToArray();

        var sourcePreventionControls = sourceCauseIds.Length == 0
            ? new List<PreventionControl>()
            : await _dbContext.PreventionControls
                .AsNoTracking()
                .Where(control => sourceCauseIds.Contains(control.FailureCauseId))
                .ToListAsync(cancellationToken);

        var sourceDetectionControls = sourceCauseIds.Length == 0
            ? new List<DetectionControl>()
            : await _dbContext.DetectionControls
                .AsNoTracking()
                .Where(control => sourceCauseIds.Contains(control.FailureCauseId))
                .ToListAsync(cancellationToken);

        var sourceRisks = sourceCauseIds.Length == 0
            ? new List<RiskAssessment>()
            : await _dbContext.RiskAssessments
                .AsNoTracking()
                .Where(risk => sourceCauseIds.Contains(risk.FailureCauseId))
                .ToListAsync(cancellationToken);

        var modesByStep = sourceModes
            .GroupBy(mode => mode.ProcessStepId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var effectsByMode = sourceEffects
            .GroupBy(effect => effect.FailureModeId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var causesByMode = sourceCauses
            .GroupBy(cause => cause.FailureModeId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var preventionByCause = sourcePreventionControls
            .GroupBy(control => control.FailureCauseId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var detectionByCause = sourceDetectionControls
            .GroupBy(control => control.FailureCauseId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var riskByCause = sourceRisks
            .ToDictionary(risk => risk.FailureCauseId);

        foreach (var sourceStep in sourceSteps)
        {
            var newStep = ProcessStep.CloneForRevision(
                targetRevisionId,
                sourceStep);

            _dbContext.ProcessSteps.Add(newStep);

            if (!modesByStep.TryGetValue(sourceStep.Id, out var modes))
            {
                continue;
            }

            foreach (var sourceMode in modes)
            {
                var newMode = FailureMode.Create(
                    newStep.Id,
                    sourceMode.Description);

                _dbContext.FailureModes.Add(newMode);

                if (effectsByMode.TryGetValue(sourceMode.Id, out var effects))
                {
                    foreach (var sourceEffect in effects)
                    {
                        _dbContext.FailureEffects.Add(
                            FailureEffect.Create(
                                newMode.Id,
                                sourceEffect.Description));
                    }
                }

                if (!causesByMode.TryGetValue(sourceMode.Id, out var causes))
                {
                    continue;
                }

                foreach (var sourceCause in causes)
                {
                    var newCause = FailureCause.Create(
                        newMode.Id,
                        sourceCause.Description);

                    _dbContext.FailureCauses.Add(newCause);

                    if (preventionByCause.TryGetValue(sourceCause.Id, out var preventionControls))
                    {
                        foreach (var sourceControl in preventionControls)
                        {
                            _dbContext.PreventionControls.Add(
                                PreventionControl.Create(
                                    newCause.Id,
                                    sourceControl.Description));
                        }
                    }

                    if (detectionByCause.TryGetValue(sourceCause.Id, out var detectionControls))
                    {
                        foreach (var sourceControl in detectionControls)
                        {
                            _dbContext.DetectionControls.Add(
                                DetectionControl.Create(
                                    newCause.Id,
                                    sourceControl.Description));
                        }
                    }

                    if (riskByCause.TryGetValue(sourceCause.Id, out var sourceRisk))
                    {
                        _dbContext.RiskAssessments.Add(
                            RiskAssessment.Create(
                                newCause.Id,
                                sourceRisk.Severity,
                                sourceRisk.Occurrence,
                                sourceRisk.Detection,
                                createdBy));
                    }
                }
            }
        }
    }
}
