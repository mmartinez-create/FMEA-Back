using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class PfmeaProcessSyncRepository
    : IPfmeaProcessSyncRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public PfmeaProcessSyncRepository(
        FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessStep>> ListTrackedByRevisionIdAsync(
        Guid revisionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessSteps
            .Where(step =>
                step.FmeaRevisionId == revisionId)
            .OrderBy(step => step.Sequence)
            .ToListAsync(cancellationToken);
    }

    public void Add(ProcessStep processStep)
    {
        _dbContext.ProcessSteps.Add(processStep);
    }
}
