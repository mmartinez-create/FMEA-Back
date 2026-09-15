using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProductProcessRepository
    : IProductProcessRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProductProcessRepository(
        FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProductProcess?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductProcesses
            .SingleOrDefaultAsync(
                process => process.Id == id,
                cancellationToken);
    }

    public Task<ProductProcess?> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductProcesses
            .AsNoTracking()
            .SingleOrDefaultAsync(
                process => process.ProjectId == projectId,
                cancellationToken);
    }

    public void Add(ProductProcess productProcess)
    {
        _dbContext.ProductProcesses.Add(productProcess);
    }
}
