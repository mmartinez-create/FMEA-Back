using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IPlantRepository
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Plant?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Plant>> ListAsync(
        CancellationToken cancellationToken = default);

    void Add(Plant plant);
}
