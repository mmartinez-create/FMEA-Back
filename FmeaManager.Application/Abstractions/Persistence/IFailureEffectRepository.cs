using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFailureEffectRepository
{
    Task<FailureEffect?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FailureEffect>> ListByFailureModeIdAsync(Guid failureModeId, CancellationToken cancellationToken = default);
    void Add(FailureEffect failureEffect);
}
