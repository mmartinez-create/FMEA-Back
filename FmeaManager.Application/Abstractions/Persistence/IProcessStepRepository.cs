using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProcessStepRepository
{
    Task<bool> ExistsBySequenceAsync(
        Guid fmeaRevisionId,
        int sequence,
        CancellationToken cancellationToken = default);

    Task<ProcessStep?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(
        Guid fmeaRevisionId,
        CancellationToken cancellationToken = default);

    void Add(ProcessStep processStep);
}
