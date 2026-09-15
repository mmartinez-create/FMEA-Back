using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class FailureModeTests
{
    [Fact]
    public void Create_WithValidData_CreatesFailureMode()
    {
        var processStepId = Guid.NewGuid();

        var failureMode = FailureMode.Create(
            processStepId,
            "  Incorrect terminal connection  ");

        Assert.NotEqual(Guid.Empty, failureMode.Id);
        Assert.Equal(processStepId, failureMode.ProcessStepId);
        Assert.Equal(
            "Incorrect terminal connection",
            failureMode.Description);
    }

    [Fact]
    public void Create_WithEmptyProcessStepId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            FailureMode.Create(
                Guid.Empty,
                "Incorrect terminal connection"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Create_WithInvalidDescription_Throws(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            FailureMode.Create(
                Guid.NewGuid(),
                description));
    }

    [Fact]
    public void UpdateDescription_WithValidData_UpdatesAndTrims()
    {
        var failureMode = FailureMode.Create(
            Guid.NewGuid(),
            "Initial");

        failureMode.UpdateDescription(
            "  Wrong polarity  ");

        Assert.Equal(
            "Wrong polarity",
            failureMode.Description);
    }
}
