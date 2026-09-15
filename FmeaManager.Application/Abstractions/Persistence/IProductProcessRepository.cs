using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProductProcessRepository
{
    Task<ProductProcess?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProductProcess?> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    void Add(ProductProcess productProcess);
}
