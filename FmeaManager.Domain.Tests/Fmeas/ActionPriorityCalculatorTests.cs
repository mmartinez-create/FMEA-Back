using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class ActionPriorityCalculatorTests
{
    [Theory]
    [InlineData(10, 10, 1, ActionPriorityLevel.High)]
    [InlineData(9, 5, 1, ActionPriorityLevel.Medium)]
    [InlineData(9, 3, 8, ActionPriorityLevel.High)]
    [InlineData(9, 3, 6, ActionPriorityLevel.Medium)]
    [InlineData(9, 3, 3, ActionPriorityLevel.Low)]
    [InlineData(8, 7, 1, ActionPriorityLevel.Medium)]
    [InlineData(8, 5, 8, ActionPriorityLevel.High)]
    [InlineData(8, 5, 3, ActionPriorityLevel.Medium)]
    [InlineData(6, 9, 8, ActionPriorityLevel.High)]
    [InlineData(6, 9, 3, ActionPriorityLevel.Medium)]
    [InlineData(6, 7, 1, ActionPriorityLevel.Low)]
    [InlineData(3, 9, 6, ActionPriorityLevel.Medium)]
    [InlineData(3, 9, 3, ActionPriorityLevel.Low)]
    [InlineData(1, 10, 10, ActionPriorityLevel.Low)]
    public void Calculate_ReturnsExpectedPriority(
        int severity,
        int occurrence,
        int detection,
        ActionPriorityLevel expected)
    {
        var actual = ActionPriorityCalculator.Calculate(
            severity,
            occurrence,
            detection);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Calculate_WithInvalidRating_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ActionPriorityCalculator.Calculate(11, 2, 2));
    }
}
