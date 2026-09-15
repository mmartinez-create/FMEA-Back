using FmeaManager.Domain.ControlPlans;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IControlPlanRepository
{
    Task<ControlPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ControlPlan?> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    void Add(ControlPlan controlPlan);
}
