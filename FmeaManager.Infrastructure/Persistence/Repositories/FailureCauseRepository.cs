using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class FailureCauseRepository : IFailureCauseRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public FailureCauseRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FailureCause?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.FailureCauses
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FailureCause>> ListByFailureModeIdAsync(
        Guid failureModeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FailureCauses
            .AsNoTracking()
            .Where(x => x.FailureModeId == failureModeId)
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public void Add(FailureCause failureCause)
    {
        _dbContext.FailureCauses.Add(failureCause);
    }
}
