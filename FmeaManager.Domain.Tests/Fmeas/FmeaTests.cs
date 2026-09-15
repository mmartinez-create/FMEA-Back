using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class FmeaTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActivePfmeaAndNormalizeNumber()
    {
        var projectId = Guid.NewGuid();

        var fmea = Fmea.Create(
            projectId,
            "  pfmea-001  ",
            "Packing Process PFMEA",
            FmeaType.Pfmea,
            "Quality Engineering",
            "cris");

        Assert.NotEqual(Guid.Empty, fmea.Id);
        Assert.Equal(projectId, fmea.ProjectId);
        Assert.Equal("PFMEA-001", fmea.Number);
        Assert.Equal("Packing Process PFMEA", fmea.Name);
        Assert.Equal(FmeaType.Pfmea, fmea.Type);
        Assert.Equal("Quality Engineering", fmea.Owner);
        Assert.True(fmea.IsActive);
        Assert.NotEqual(default, fmea.CreatedAt);
        Assert.Equal("cris", fmea.CreatedBy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyNumber_ShouldThrowArgumentException(string number)
    {
        var action = () => Fmea.Create(
            Guid.NewGuid(),
            number,
            "Packing Process PFMEA",
            FmeaType.Pfmea,
            "Quality Engineering",
            "cris");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("number", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyProject_ShouldThrowArgumentException()
    {
        var action = () => Fmea.Create(
            Guid.Empty,
            "PFMEA-001",
            "Packing Process PFMEA",
            FmeaType.Pfmea,
            "Quality Engineering",
            "cris");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("projectId", exception.ParamName);
    }

    [Fact]
    public void UpdateInformation_WhenFmeaIsInactive_ShouldThrowInvalidOperationException()
    {
        var fmea = CreateFmea();
        fmea.Deactivate();

        var action = () => fmea.UpdateInformation(
            "Updated PFMEA",
            "Updated Owner");

        Assert.Throws<InvalidOperationException>(action);
    }

    private static Fmea CreateFmea()
    {
        return Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-001",
            "Packing Process PFMEA",
            FmeaType.Pfmea,
            "Quality Engineering",
            "cris");
    }
}
