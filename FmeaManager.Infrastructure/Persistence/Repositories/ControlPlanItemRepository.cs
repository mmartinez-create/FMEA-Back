using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ControlPlans;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ControlPlanItemRepository : IControlPlanItemRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ControlPlanItemRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ControlPlanItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.ControlPlanItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ControlPlanItem>> ListByControlPlanIdAsync(
        Guid controlPlanId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.ControlPlanItems
            .Where(x => x.ControlPlanId == controlPlanId)
            .OrderBy(x => x.ProcessStepSequenceSnapshot)
            .ThenBy(x => x.CharacteristicNumber)
            .ToListAsync(cancellationToken);

    public void Add(ControlPlanItem item) => _dbContext.ControlPlanItems.Add(item);
}
