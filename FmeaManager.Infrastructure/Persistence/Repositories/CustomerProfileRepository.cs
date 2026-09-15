using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.CustomerProfiles;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence.Repositories;

public sealed class CustomerProfileRepository : ICustomerProfileRepository
{
    private readonly FmeaManagerDbContext _dbContext;

    public CustomerProfileRepository(FmeaManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CustomerProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.CustomerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                profile => profile.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerProfile>> ListActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CustomerProfiles
            .AsNoTracking()
            .Where(profile => profile.IsActive)
            .OrderBy(profile => profile.Name)
            .ToListAsync(cancellationToken);
    }
}
