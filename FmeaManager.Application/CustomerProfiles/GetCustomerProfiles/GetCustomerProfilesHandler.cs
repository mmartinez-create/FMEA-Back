using FmeaManager.Application.Abstractions.Persistence;

namespace FmeaManager.Application.CustomerProfiles.GetCustomerProfiles;

public sealed class GetCustomerProfilesHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;

    public GetCustomerProfilesHandler(ICustomerProfileRepository customerProfileRepository)
    {
        _customerProfileRepository = customerProfileRepository;
    }

    public async Task<IReadOnlyList<CustomerProfileSummary>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var profiles = await _customerProfileRepository.ListActiveAsync(cancellationToken);

        return profiles
            .Select(profile => new CustomerProfileSummary(
                profile.Id,
                profile.Code,
                profile.Name,
                profile.Description))
            .ToList();
    }
}
