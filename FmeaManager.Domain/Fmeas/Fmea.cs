namespace FmeaManager.Domain.Fmeas;

public sealed class Fmea
{
    private Fmea()
    {
    }

    private Fmea(
        Guid id,
        Guid projectId,
        string number,
        string name,
        FmeaType type,
        string owner,
        DateTime createdAt,
        string createdBy)
    {
        Id = id;
        ProjectId = projectId;
        Number = number;
        Name = name;
        Type = type;
        Owner = owner;
        IsActive = true;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public FmeaType Type { get; private set; }

    public string Owner { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? UpdatedAt { get; private set; }

    public static Fmea Create(
        Guid projectId,
        string number,
        string name,
        FmeaType type,
        string owner,
        string createdBy)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Project is required.",
                nameof(projectId));
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            throw new ArgumentException(
                "FMEA number is required.",
                nameof(number));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "FMEA name is required.",
                nameof(name));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "FMEA type is invalid.");
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException(
                "FMEA owner is required.",
                nameof(owner));
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException(
                "Created by is required.",
                nameof(createdBy));
        }

        return new Fmea(
            Guid.NewGuid(),
            projectId,
            number.Trim().ToUpperInvariant(),
            name.Trim(),
            type,
            owner.Trim(),
            DateTime.UtcNow,
            createdBy.Trim());
    }

    public void UpdateInformation(string name, string owner)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "FMEA name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException(
                "FMEA owner is required.",
                nameof(owner));
        }

        Name = name.Trim();
        Owner = owner.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        EnsureActive();
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("An inactive FMEA cannot be modified.");
        }
    }
}
