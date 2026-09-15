using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFailureCauseRepository
{
    Task<FailureCause?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FailureCause>> ListByFailureModeIdAsync(Guid failureModeId, CancellationToken cancellationToken = default);
    void Add(FailureCause failureCause);
}
