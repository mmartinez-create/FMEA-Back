using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProductionLineRepository
{
    Task<bool> ExistsByCodeAsync(
        Guid plantId,
        string code,
        CancellationToken cancellationToken = default);

    Task<ProductionLine?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionLine>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionLine>> ListByPlantIdAsync(
        Guid plantId,
        CancellationToken cancellationToken = default);

    void Add(ProductionLine productionLine);
}
