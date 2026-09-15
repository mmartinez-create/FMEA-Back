using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class PreventionControlTests
{
    [Fact]
    public void Create_WithValidData_CreatesPreventionControl()
    {
        var failureCauseId = Guid.NewGuid();

        var control = PreventionControl.Create(
            failureCauseId,
            "  Poka-yoke assembly fixture  ");

        Assert.NotEqual(Guid.Empty, control.Id);
        Assert.Equal(failureCauseId, control.FailureCauseId);
        Assert.Equal(
            "Poka-yoke assembly fixture",
            control.Description);
    }

    [Fact]
    public void Create_WithEmptyFailureCauseId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            PreventionControl.Create(
                Guid.Empty,
                "Assembly fixture"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_Throws(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            PreventionControl.Create(
                Guid.NewGuid(),
                description));
    }

    [Fact]
    public void UpdateDescription_WithValidData_UpdatesAndTrims()
    {
        var control = PreventionControl.Create(
            Guid.NewGuid(),
            "Initial");

        control.UpdateDescription(
            "  Work instruction and fixture  ");

        Assert.Equal(
            "Work instruction and fixture",
            control.Description);
    }
}
