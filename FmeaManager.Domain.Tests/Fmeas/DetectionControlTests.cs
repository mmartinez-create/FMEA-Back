using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class DetectionControlTests
{
    [Fact]
    public void Create_WithValidData_CreatesDetectionControl()
    {
        var failureCauseId = Guid.NewGuid();

        var control = DetectionControl.Create(
            failureCauseId,
            "  Vision inspection  ");

        Assert.NotEqual(Guid.Empty, control.Id);
        Assert.Equal(failureCauseId, control.FailureCauseId);
        Assert.Equal(
            "Vision inspection",
            control.Description);
    }

    [Fact]
    public void Create_WithEmptyFailureCauseId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            DetectionControl.Create(
                Guid.Empty,
                "Vision inspection"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_Throws(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            DetectionControl.Create(
                Guid.NewGuid(),
                description));
    }

    [Fact]
    public void UpdateDescription_WithValidData_UpdatesAndTrims()
    {
        var control = DetectionControl.Create(
            Guid.NewGuid(),
            "Initial");

        control.UpdateDescription(
            "  End-of-line electrical test  ");

        Assert.Equal(
            "End-of-line electrical test",
            control.Description);
    }
}
