using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class FailureEffectTests
{
    [Fact]
    public void Create_WithValidData_CreatesFailureEffect()
    {
        var failureModeId = Guid.NewGuid();

        var effect = FailureEffect.Create(
            failureModeId,
            "  Vehicle cannot start  ");

        Assert.NotEqual(Guid.Empty, effect.Id);
        Assert.Equal(failureModeId, effect.FailureModeId);
        Assert.Equal("Vehicle cannot start", effect.Description);
    }

    [Fact]
    public void Create_WithEmptyFailureModeId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            FailureEffect.Create(
                Guid.Empty,
                "Vehicle cannot start"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_Throws(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            FailureEffect.Create(
                Guid.NewGuid(),
                description));
    }

    [Fact]
    public void UpdateDescription_WithValidData_UpdatesAndTrims()
    {
        var effect = FailureEffect.Create(
            Guid.NewGuid(),
            "Initial");

        effect.UpdateDescription(
            "  Loss of vehicle function  ");

        Assert.Equal(
            "Loss of vehicle function",
            effect.Description);
    }
}
