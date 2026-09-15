using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ControlPlans;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ControlPlanRepository : IControlPlanRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ControlPlanRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ControlPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.ControlPlans.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<ControlPlan?> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        _dbContext.ControlPlans.SingleOrDefaultAsync(x => x.ProjectId == projectId, cancellationToken);

    public void Add(ControlPlan controlPlan) => _dbContext.ControlPlans.Add(controlPlan);
}
