namespace FmeaManager.Application.CustomerProfiles.GetCustomerProfiles;

public sealed record CustomerProfileSummary(
    Guid Id,
    string Code,
    string Name,
    string? Description);
