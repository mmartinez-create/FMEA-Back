using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class ProcessStepTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAndNormalizeProcessStep()
    {
        var revisionId = Guid.NewGuid();

        var step = ProcessStep.Create(
            revisionId,
            10,
            "  Packing  ",
            "  Pack finished product  ",
            "  Maintain traceability  ");

        Assert.NotEqual(Guid.Empty, step.Id);
        Assert.Equal(revisionId, step.FmeaRevisionId);
        Assert.Equal(10, step.Sequence);
        Assert.Equal("Packing", step.Name);
        Assert.Equal("Pack finished product", step.Function);
        Assert.Equal("Maintain traceability", step.Requirement);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidSequence_ShouldThrowArgumentOutOfRangeException(int sequence)
    {
        var action = () => ProcessStep.Create(
            Guid.NewGuid(),
            sequence,
            "Packing");

        var exception = Assert.Throws<ArgumentOutOfRangeException>(action);

        Assert.Equal("sequence", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyRevision_ShouldThrowArgumentException()
    {
        var action = () => ProcessStep.Create(
            Guid.Empty,
            10,
            "Packing");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("fmeaRevisionId", exception.ParamName);
    }

    [Fact]
    public void UpdateInformation_ShouldReplaceEditableValues()
    {
        var step = ProcessStep.Create(
            Guid.NewGuid(),
            10,
            "Packing");

        step.UpdateInformation(
            20,
            "Final Packing",
            "Pack finished product",
            "Correct quantity and traceability");

        Assert.Equal(20, step.Sequence);
        Assert.Equal("Final Packing", step.Name);
        Assert.Equal("Pack finished product", step.Function);
        Assert.Equal("Correct quantity and traceability", step.Requirement);
    }
}
