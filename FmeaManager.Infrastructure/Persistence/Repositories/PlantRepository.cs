using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class PlantRepository : IPlantRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public PlantRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Plants.AnyAsync(
            plant => plant.Code == code,
            cancellationToken);
    }

    public Task<Plant?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Plants
            .AsNoTracking()
            .SingleOrDefaultAsync(
                plant => plant.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Plant>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Plants
            .AsNoTracking()
            .OrderBy(plant => plant.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(Plant plant)
    {
        _dbContext.Plants.Add(plant);
    }
}
