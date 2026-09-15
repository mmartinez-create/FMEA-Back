using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.Tests.ProductStructure;

public sealed class ProductProcessTests
{
    [Fact]
    public void CreateProcess_WithValidData_NormalizesFields()
    {
        var process = ProductProcess.Create(
            Guid.NewGuid(),
            " Main Assembly Process ",
            " Manufacturing flow ",
            " QUALITY.USER ");

        Assert.Equal("Main Assembly Process", process.Name);
        Assert.Equal("Manufacturing flow", process.Description);
        Assert.Equal("quality.user", process.CreatedBy);
        Assert.True(process.IsActive);
    }

    [Fact]
    public void CreateStep_WithValidData_NormalizesAndKeepsSequence()
    {
        var step = ProductProcessStep.Create(
            Guid.NewGuid(),
            20,
            " Terminal Assembly ",
            " Connect terminal ",
            " Correct polarity ",
            " QUALITY.USER ");

        Assert.Equal(20, step.Sequence);
        Assert.Equal("Terminal Assembly", step.Name);
        Assert.Equal("Connect terminal", step.Function);
        Assert.Equal("Correct polarity", step.Requirement);
        Assert.Equal("quality.user", step.CreatedBy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CreateStep_WithInvalidSequence_Throws(int sequence)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ProductProcessStep.Create(
                Guid.NewGuid(),
                sequence,
                "Assembly",
                null,
                null,
                "user"));
    }
}
