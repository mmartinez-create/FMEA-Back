namespace FmeaManager.Domain.CustomerProfiles;

public sealed class CustomerProfile
{
    private CustomerProfile()
    {
    }

    private CustomerProfile(
        Guid id,
        string code,
        string name,
        string? description)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public static CustomerProfile Create(
        string code,
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Customer profile code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer profile name is required.",
                nameof(name));
        }

        return new CustomerProfile(
            Guid.NewGuid(),
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            description?.Trim());
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
