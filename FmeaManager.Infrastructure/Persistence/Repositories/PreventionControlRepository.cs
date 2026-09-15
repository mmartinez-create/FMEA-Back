using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class PreventionControlRepository : IPreventionControlRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public PreventionControlRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PreventionControl?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.PreventionControls
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PreventionControl>> ListByFailureCauseIdAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PreventionControls
            .AsNoTracking()
            .Where(x => x.FailureCauseId == failureCauseId)
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public void Add(PreventionControl preventionControl)
    {
        _dbContext.PreventionControls.Add(preventionControl);
    }
}
