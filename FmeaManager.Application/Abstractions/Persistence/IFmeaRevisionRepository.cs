using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFmeaRevisionRepository
{
    Task<FmeaRevision?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(
        Guid fmeaId,
        CancellationToken cancellationToken = default);

    void Add(FmeaRevision revision);
}
