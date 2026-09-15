using FmeaManager.Domain.CustomerProfiles;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface ICustomerProfileRepository
{
    Task<CustomerProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerProfile>> ListActiveAsync(
        CancellationToken cancellationToken = default);
}
