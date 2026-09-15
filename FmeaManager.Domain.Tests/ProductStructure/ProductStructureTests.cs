using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.Tests.ProductStructure;

public sealed class ProductStructureTests
{
    [Fact]
    public void Plant_Create_NormalizesCodeAndName()
    {
        var plant = Plant.Create(" mx-tol ", " Toluca Plant ");

        Assert.Equal("MX-TOL", plant.Code);
        Assert.Equal("Toluca Plant", plant.Name);
        Assert.True(plant.IsActive);
    }

    [Fact]
    public void ProductionLine_Create_RequiresPlant()
    {
        Assert.Throws<ArgumentException>(() =>
            ProductionLine.Create(
                Guid.Empty,
                "ASSY-1",
                "Assembly Line 1"));
    }

    [Fact]
    public void Product_Create_NormalizesOptionalPartNumber()
    {
        var product = Product.Create(
            Guid.NewGuid(),
            "bat-001",
            "12V Battery",
            "  PN-001  ");

        Assert.Equal("BAT-001", product.Code);
        Assert.Equal("12V Battery", product.Name);
        Assert.Equal("PN-001", product.PartNumber);
    }
}
