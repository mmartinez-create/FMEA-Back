namespace FmeaManager.Domain.ProductStructure;

public sealed class ProductProcessStep
{
    private ProductProcessStep()
    {
    }

    private ProductProcessStep(
        Guid id,
        Guid productProcessId,
        int sequence,
        string name,
        string? function,
        string? requirement,
        string createdBy,
        DateTime createdAt)
    {
        Id = id;
        ProductProcessId = productProcessId;
        Sequence = sequence;
        Name = name;
        Function = function;
        Requirement = requirement;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ProductProcessId { get; private set; }

    public int Sequence { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Function { get; private set; }

    public string? Requirement { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }

    public static ProductProcessStep Create(
        Guid productProcessId,
        int sequence,
        string name,
        string? function,
        string? requirement,
        string createdBy)
    {
        if (productProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product process is required.",
                nameof(productProcessId));
        }

        ValidateSequence(sequence);
        ValidateName(name);
        ValidateUser(createdBy, nameof(createdBy));

        return new ProductProcessStep(
            Guid.NewGuid(),
            productProcessId,
            sequence,
            name.Trim(),
            NormalizeOptional(function),
            NormalizeOptional(requirement),
            createdBy.Trim().ToLowerInvariant(),
            DateTime.UtcNow);
    }

    public void UpdateInformation(
        int sequence,
        string name,
        string? function,
        string? requirement,
        string updatedBy)
    {
        ValidateSequence(sequence);
        ValidateName(name);
        ValidateUser(updatedBy, nameof(updatedBy));

        Sequence = sequence;
        Name = name.Trim();
        Function = NormalizeOptional(function);
        Requirement = NormalizeOptional(requirement);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy.Trim().ToLowerInvariant();
    }

    private static void ValidateSequence(int sequence)
    {
        if (sequence <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequence),
                sequence,
                "Process step sequence must be greater than zero.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Process step name is required.",
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
