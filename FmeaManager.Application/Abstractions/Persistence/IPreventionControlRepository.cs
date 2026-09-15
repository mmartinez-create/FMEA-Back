using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IPreventionControlRepository
{
    Task<PreventionControl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PreventionControl>> ListByFailureCauseIdAsync(Guid failureCauseId, CancellationToken cancellationToken = default);
    void Add(PreventionControl preventionControl);
}
