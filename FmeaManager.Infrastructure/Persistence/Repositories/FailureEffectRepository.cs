using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class FailureEffectRepository : IFailureEffectRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public FailureEffectRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FailureEffect?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.FailureEffects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FailureEffect>> ListByFailureModeIdAsync(
        Guid failureModeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FailureEffects
            .AsNoTracking()
            .Where(x => x.FailureModeId == failureModeId)
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public void Add(FailureEffect failureEffect)
    {
        _dbContext.FailureEffects.Add(failureEffect);
    }
}
