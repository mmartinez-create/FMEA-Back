using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class FmeaRepository : IFmeaRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public FmeaRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string number,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Fmeas.AnyAsync(
            fmea => fmea.ProjectId == projectId
                && fmea.Number == number,
            cancellationToken);
    }

    public Task<Fmea?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Fmeas
            .AsNoTracking()
            .SingleOrDefaultAsync(
                fmea => fmea.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Fmeas
            .AsNoTracking()
            .Where(fmea => fmea.ProjectId == projectId)
            .OrderBy(fmea => fmea.Number)
            .ToListAsync(cancellationToken);
    }

    public void Add(Fmea fmea)
    {
        _dbContext.Fmeas.Add(fmea);
    }
}
