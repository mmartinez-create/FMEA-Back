namespace FmeaManager.Domain.ProductStructure;

public sealed class Product
{
    private Product()
    {
    }

    private Product(
        Guid id,
        Guid productionLineId,
        string code,
        string name,
        string? partNumber)
    {
        Id = id;
        ProductionLineId = productionLineId;
        Code = code;
        Name = name;
        PartNumber = partNumber;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid ProductionLineId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? PartNumber { get; private set; }

    public bool IsActive { get; private set; }

    public static Product Create(
        Guid productionLineId,
        string code,
        string name,
        string? partNumber)
    {
        if (productionLineId == Guid.Empty)
        {
            throw new ArgumentException(
                "Production line is required.",
                nameof(productionLineId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Product code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        return new Product(
            Guid.NewGuid(),
            productionLineId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            NormalizeOptional(partNumber));
    }

    public void UpdateInformation(
        string name,
        string? partNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        Name = name.Trim();
        PartNumber = NormalizeOptional(partNumber);
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
