using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProcessStepRepository : IProcessStepRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProcessStepRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsBySequenceAsync(
        Guid fmeaRevisionId,
        int sequence,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProcessSteps.AnyAsync(
            step => step.FmeaRevisionId == fmeaRevisionId
                && step.Sequence == sequence,
            cancellationToken);
    }

    public Task<ProcessStep?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProcessSteps
            .AsNoTracking()
            .SingleOrDefaultAsync(
                step => step.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(
        Guid fmeaRevisionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessSteps
            .AsNoTracking()
            .Where(step => step.FmeaRevisionId == fmeaRevisionId)
            .OrderBy(step => step.Sequence)
            .ToListAsync(cancellationToken);
    }

    public void Add(ProcessStep processStep)
    {
        _dbContext.ProcessSteps.Add(processStep);
    }
}
