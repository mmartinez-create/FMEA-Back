using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProductProcessStepRepository
    : IProductProcessStepRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProductProcessStepRepository(
        FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProductProcessStep?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductProcessSteps
            .SingleOrDefaultAsync(
                step => step.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProductProcessStep>> ListByProductProcessIdAsync(
        Guid productProcessId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductProcessSteps
            .AsNoTracking()
            .Where(step =>
                step.ProductProcessId == productProcessId)
            .OrderBy(step => step.Sequence)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsBySequenceAsync(
        Guid productProcessId,
        int sequence,
        Guid? excludingStepId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductProcessSteps.AnyAsync(
            step =>
                step.ProductProcessId == productProcessId &&
                step.Sequence == sequence &&
                (!excludingStepId.HasValue ||
                 step.Id != excludingStepId.Value),
            cancellationToken);
    }

    public void Add(ProductProcessStep step)
    {
        _dbContext.ProductProcessSteps.Add(step);
    }
}
