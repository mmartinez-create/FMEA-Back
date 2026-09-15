using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class DetectionControlRepository : IDetectionControlRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public DetectionControlRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<DetectionControl?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.DetectionControls
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DetectionControl>> ListByFailureCauseIdAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.DetectionControls
            .AsNoTracking()
            .Where(x => x.FailureCauseId == failureCauseId)
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public void Add(DetectionControl detectionControl)
    {
        _dbContext.DetectionControls.Add(detectionControl);
    }
}
