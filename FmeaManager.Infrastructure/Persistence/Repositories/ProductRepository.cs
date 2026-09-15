using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProductRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByCodeAsync(
        Guid productionLineId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.AnyAsync(
            product =>
                product.ProductionLineId == productionLineId &&
                product.Code == code,
            cancellationToken);
    }

    public Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListByProductionLineIdAsync(
        Guid productionLineId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.ProductionLineId == productionLineId)
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(Product product)
    {
        _dbContext.Products.Add(product);
    }
}
