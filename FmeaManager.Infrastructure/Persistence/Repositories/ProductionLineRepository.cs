using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class ProductionLineRepository
    : IProductionLineRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public ProductionLineRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByCodeAsync(
        Guid plantId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductionLines.AnyAsync(
            line => line.PlantId == plantId && line.Code == code,
            cancellationToken);
    }

    public Task<ProductionLine?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductionLines
            .AsNoTracking()
            .SingleOrDefaultAsync(
                line => line.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionLine>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductionLines
            .AsNoTracking()
            .OrderBy(line => line.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionLine>> ListByPlantIdAsync(
        Guid plantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductionLines
            .AsNoTracking()
            .Where(line => line.PlantId == plantId)
            .OrderBy(line => line.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(ProductionLine productionLine)
    {
        _dbContext.ProductionLines.Add(productionLine);
    }
}
