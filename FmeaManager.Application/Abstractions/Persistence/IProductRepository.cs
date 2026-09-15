using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<bool> ExistsByCodeAsync(
        Guid productionLineId,
        string code,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListByProductionLineIdAsync(
        Guid productionLineId,
        CancellationToken cancellationToken = default);

    void Add(Product product);
}
