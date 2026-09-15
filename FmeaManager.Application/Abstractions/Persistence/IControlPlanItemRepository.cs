using FmeaManager.Domain.ControlPlans;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IControlPlanItemRepository
{
    Task<ControlPlanItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ControlPlanItem>> ListByControlPlanIdAsync(
        Guid controlPlanId,
        CancellationToken cancellationToken = default);
    void Add(ControlPlanItem item);
}
