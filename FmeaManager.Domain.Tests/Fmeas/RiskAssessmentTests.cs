using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class RiskAssessmentTests
{
    [Fact]
    public void Create_WithValidRatings_CalculatesRpn()
    {
        var assessment = RiskAssessment.Create(
            Guid.NewGuid(),
            severity: 9,
            occurrence: 4,
            detection: 3,
            createdBy: "cris");

        Assert.Equal(9, assessment.Severity);
        Assert.Equal(4, assessment.Occurrence);
        Assert.Equal(3, assessment.Detection);
        Assert.Equal(108, assessment.Rpn);
        Assert.Equal("cris", assessment.CreatedBy);
    }

    [Theory]
    [InlineData(0, 5, 5)]
    [InlineData(11, 5, 5)]
    [InlineData(5, 0, 5)]
    [InlineData(5, 11, 5)]
    [InlineData(5, 5, 0)]
    [InlineData(5, 5, 11)]
    public void Create_WithRatingOutsideOneToTen_Throws(
        int severity,
        int occurrence,
        int detection)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RiskAssessment.Create(
                Guid.NewGuid(),
                severity,
                occurrence,
                detection,
                "cris"));
    }

    [Fact]
    public void UpdateRatings_RecalculatesRpnAndAuditInformation()
    {
        var assessment = RiskAssessment.Create(
            Guid.NewGuid(),
            5,
            5,
            5,
            "creator");

        assessment.UpdateRatings(
            10,
            2,
            4,
            "reviewer");

        Assert.Equal(80, assessment.Rpn);
        Assert.Equal("reviewer", assessment.UpdatedBy);
        Assert.NotNull(assessment.UpdatedAt);
    }

    [Fact]
    public void Create_WithEmptyFailureCause_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            RiskAssessment.Create(
                Guid.Empty,
                5,
                5,
                5,
                "cris"));
    }
}
