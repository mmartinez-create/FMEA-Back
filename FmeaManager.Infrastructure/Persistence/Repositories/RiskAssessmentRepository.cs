using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class RiskAssessmentRepository : IRiskAssessmentRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public RiskAssessmentRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RiskAssessment?> GetByFailureCauseIdAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.RiskAssessments
            .SingleOrDefaultAsync(
                x => x.FailureCauseId == failureCauseId,
                cancellationToken);
    }

    public void Add(RiskAssessment riskAssessment)
    {
        _dbContext.RiskAssessments.Add(riskAssessment);
    }
}
