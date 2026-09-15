namespace FmeaManager.Domain.Projects;

public sealed class Project
{
    private Project()
    {
    }

    private Project(
        Guid id,
        string code,
        string name,
        string? product,
        string? plant,
        Guid customerProfileId,
        string owner,
        DateTime createdAt,
        string createdBy)
    {
        Id = id;
        Code = code;
        Name = name;
        Product = product;
        Plant = plant;
        CustomerProfileId = customerProfileId;
        Owner = owner;
        Status = ProjectStatus.Draft;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Product { get; private set; }

    public Guid? ProductId { get; private set; }

    public string? Plant { get; private set; }

    public Guid CustomerProfileId { get; private set; }

    public string Owner { get; private set; } = string.Empty;

    public ProjectStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public static Project Create(
        string code,
        string name,
        string? product,
        string? plant,
        Guid customerProfileId,
        string owner,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Project code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Project name is required.",
                nameof(name));
        }

        if (customerProfileId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer profile is required.",
                nameof(customerProfileId));
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException(
                "Project owner is required.",
                nameof(owner));
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException(
                "Created by is required.",
                nameof(createdBy));
        }

        return new Project(
            Guid.NewGuid(),
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            NormalizeOptional(product),
            NormalizeOptional(plant),
            customerProfileId,
            owner.Trim(),
            DateTime.UtcNow,
            createdBy.Trim());
    }

    public void UpdateInformation(
        string name,
        string? product,
        string? plant,
        string owner)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Project name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException(
                "Project owner is required.",
                nameof(owner));
        }

        Name = name.Trim();
        Product = NormalizeOptional(product);
        Plant = NormalizeOptional(plant);
        Owner = owner.Trim();
        UpdatedAt = DateTime.UtcNow;
    }


    public void AssignToProduct(Guid productId)
    {
        EnsureEditable();

        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product is required.",
                nameof(productId));
        }

        ProductId = productId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        EnsureStatus(ProjectStatus.Draft, "Only a draft project can be activated.");
        Status = ProjectStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        EnsureStatus(ProjectStatus.Active, "Only an active project can be completed.");
        Status = ProjectStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        EnsureStatus(ProjectStatus.Completed, "Only a completed project can be archived.");
        Status = ProjectStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureEditable()
    {
        if (Status == ProjectStatus.Archived)
        {
            throw new InvalidOperationException("An archived project cannot be modified.");
        }
    }

    private void EnsureStatus(ProjectStatus requiredStatus, string message)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
