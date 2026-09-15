namespace FmeaManager.Domain.ProductStructure;

public sealed class ProductProcess
{
    private ProductProcess()
    {
    }

    private ProductProcess(
        Guid id,
        Guid projectId,
        string name,
        string? description,
        string createdBy,
        DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }

    public static ProductProcess Create(
        Guid projectId,
        string name,
        string? description,
        string createdBy)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Project is required.",
                nameof(projectId));
        }

        ValidateName(name);
        ValidateUser(createdBy, nameof(createdBy));

        return new ProductProcess(
            Guid.NewGuid(),
            projectId,
            name.Trim(),
            NormalizeOptional(description),
            createdBy.Trim().ToLowerInvariant(),
            DateTime.UtcNow);
    }

    public void UpdateInformation(
        string name,
        string? description,
        string updatedBy)
    {
        ValidateName(name);
        ValidateUser(updatedBy, nameof(updatedBy));

        Name = name.Trim();
        Description = NormalizeOptional(description);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy.Trim().ToLowerInvariant();
    }

    public void Deactivate(string updatedBy)
    {
        ValidateUser(updatedBy, nameof(updatedBy));

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy.Trim().ToLowerInvariant();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product process name is required.",
                nameof(name));
        }
    }

    private static void ValidateUser(string user, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            throw new ArgumentException(
                "User is required.",
                parameterName);
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
