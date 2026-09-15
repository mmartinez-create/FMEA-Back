using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.Fmeas;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class FmeaRevisionRepository : IFmeaRevisionRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public FmeaRevisionRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FmeaRevision?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.FmeaRevisions
            .SingleOrDefaultAsync(
                revision => revision.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(
        Guid fmeaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FmeaRevisions
            .AsNoTracking()
            .Where(revision => revision.FmeaId == fmeaId)
            .OrderByDescending(revision => revision.RevisionNumber)
            .ToListAsync(cancellationToken);
    }

    public void Add(FmeaRevision revision)
    {
        _dbContext.FmeaRevisions.Add(revision);
    }
}
