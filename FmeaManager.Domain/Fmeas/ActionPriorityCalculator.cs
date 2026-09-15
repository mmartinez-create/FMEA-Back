namespace FmeaManager.Domain.Fmeas;

public static class ActionPriorityCalculator
{
    public static ActionPriorityLevel Calculate(
        int severity,
        int occurrence,
        int detection)
    {
        ValidateRating(severity, nameof(severity));
        ValidateRating(occurrence, nameof(occurrence));
        ValidateRating(detection, nameof(detection));

        if (severity >= 9)
        {
            return CalculateSeverityVeryHigh(
                occurrence,
                detection);
        }

        if (severity >= 7)
        {
            return CalculateSeverityHigh(
                occurrence,
                detection);
        }

        if (severity >= 4)
        {
            return CalculateSeverityModerate(
                occurrence,
                detection);
        }

        if (severity >= 2)
        {
            return CalculateSeverityLow(
                occurrence,
                detection);
        }

        return ActionPriorityLevel.Low;
    }

    private static ActionPriorityLevel CalculateSeverityVeryHigh(
        int occurrence,
        int detection)
    {
        if (occurrence >= 6)
        {
            return ActionPriorityLevel.High;
        }

        if (occurrence >= 4)
        {
            return detection == 1
                ? ActionPriorityLevel.Medium
                : ActionPriorityLevel.High;
        }

        if (occurrence >= 2)
        {
            if (detection >= 7)
            {
                return ActionPriorityLevel.High;
            }

            if (detection >= 5)
            {
                return ActionPriorityLevel.Medium;
            }

            return ActionPriorityLevel.Low;
        }

        return ActionPriorityLevel.Low;
    }

    private static ActionPriorityLevel CalculateSeverityHigh(
        int occurrence,
        int detection)
    {
        if (occurrence >= 8)
        {
            return ActionPriorityLevel.High;
        }

        if (occurrence >= 6)
        {
            return detection == 1
                ? ActionPriorityLevel.Medium
                : ActionPriorityLevel.High;
        }

        if (occurrence >= 4)
        {
            return detection >= 7
                ? ActionPriorityLevel.High
                : ActionPriorityLevel.Medium;
        }

        if (occurrence >= 2)
        {
            return detection >= 5
                ? ActionPriorityLevel.Medium
                : ActionPriorityLevel.Low;
        }

        return ActionPriorityLevel.Low;
    }

    private static ActionPriorityLevel CalculateSeverityModerate(
        int occurrence,
        int detection)
    {
        if (occurrence >= 8)
        {
            if (detection >= 5)
            {
                return ActionPriorityLevel.High;
            }

            return ActionPriorityLevel.Medium;
        }

        if (occurrence >= 6)
        {
            return detection == 1
                ? ActionPriorityLevel.Low
                : ActionPriorityLevel.Medium;
        }

        if (occurrence >= 4)
        {
            return detection >= 7
                ? ActionPriorityLevel.Medium
                : ActionPriorityLevel.Low;
        }

        return ActionPriorityLevel.Low;
    }

    private static ActionPriorityLevel CalculateSeverityLow(
        int occurrence,
        int detection)
    {
        if (occurrence >= 8)
        {
            return detection >= 5
                ? ActionPriorityLevel.Medium
                : ActionPriorityLevel.Low;
        }

        return ActionPriorityLevel.Low;
    }

    private static void ValidateRating(
        int value,
        string parameterName)
    {
        if (value is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "FMEA ratings must be between 1 and 10.");
        }
    }
}
