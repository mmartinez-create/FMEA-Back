using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IPfmeaProcessSyncRepository
{
    Task<IReadOnlyList<ProcessStep>> ListTrackedByRevisionIdAsync(
        Guid revisionId,
        CancellationToken cancellationToken = default);

    void Add(ProcessStep processStep);
}
