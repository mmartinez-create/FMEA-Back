using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFmeaRepository
{
    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string number,
        CancellationToken cancellationToken = default);

    Task<Fmea?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    void Add(Fmea fmea);
}
