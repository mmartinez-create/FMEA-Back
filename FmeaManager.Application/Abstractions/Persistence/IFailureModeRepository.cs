using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFailureModeRepository
{
    Task<FailureMode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FailureMode>> ListByProcessStepIdAsync(Guid processStepId, CancellationToken cancellationToken = default);
    void Add(FailureMode failureMode);
}
