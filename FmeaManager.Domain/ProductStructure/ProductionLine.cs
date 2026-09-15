namespace FmeaManager.Domain.ProductStructure;

public sealed class ProductionLine
{
    private ProductionLine()
    {
    }

    private ProductionLine(
        Guid id,
        Guid plantId,
        string code,
        string name)
    {
        Id = id;
        PlantId = plantId;
        Code = code;
        Name = name;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid PlantId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static ProductionLine Create(
        Guid plantId,
        string code,
        string name)
    {
        if (plantId == Guid.Empty)
        {
            throw new ArgumentException("Plant is required.", nameof(plantId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Production line code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Production line name is required.",
                nameof(name));
        }

        return new ProductionLine(
            Guid.NewGuid(),
            plantId,
            code.Trim().ToUpperInvariant(),
            name.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Production line name is required.",
                nameof(name));
        }

        Name = name.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
