using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProductProcessStepRepository
{
    Task<ProductProcessStep?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductProcessStep>> ListByProductProcessIdAsync(
        Guid productProcessId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySequenceAsync(
        Guid productProcessId,
        int sequence,
        Guid? excludingStepId = null,
        CancellationToken cancellationToken = default);

    void Add(ProductProcessStep step);
}
