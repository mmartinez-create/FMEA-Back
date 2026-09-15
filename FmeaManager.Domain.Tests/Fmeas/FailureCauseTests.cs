using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class FailureCauseTests
{
    [Fact]
    public void Create_WithValidData_CreatesFailureCause()
    {
        var failureModeId = Guid.NewGuid();

        var cause = FailureCause.Create(
            failureModeId,
            "  Terminal installed in wrong position  ");

        Assert.NotEqual(Guid.Empty, cause.Id);
        Assert.Equal(failureModeId, cause.FailureModeId);
        Assert.Equal(
            "Terminal installed in wrong position",
            cause.Description);
    }

    [Fact]
    public void Create_WithEmptyFailureModeId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            FailureCause.Create(
                Guid.Empty,
                "Wrong position"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_Throws(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            FailureCause.Create(
                Guid.NewGuid(),
                description));
    }

    [Fact]
    public void UpdateDescription_WithValidData_UpdatesAndTrims()
    {
        var cause = FailureCause.Create(
            Guid.NewGuid(),
            "Initial");

        cause.UpdateDescription(
            "  Incorrect fixture setup  ");

        Assert.Equal(
            "Incorrect fixture setup",
            cause.Description);
    }
}
