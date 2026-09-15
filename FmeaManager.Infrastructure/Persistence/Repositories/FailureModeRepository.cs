using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class FailureModeRepository : IFailureModeRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public FailureModeRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FailureMode?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.FailureModes
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FailureMode>> ListByProcessStepIdAsync(
        Guid processStepId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FailureModes
            .AsNoTracking()
            .Where(x => x.ProcessStepId == processStepId)
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public void Add(FailureMode failureMode)
    {
        _dbContext.FailureModes.Add(failureMode);
    }
}
