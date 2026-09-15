using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.Tests.ControlPlans;

public sealed class ControlPlanTests
{
    [Fact]
    public void Create_StartsAsDraft()
    {
        var plan = ControlPlan.Create(
            Guid.NewGuid(), "CP-001", "Battery Control Plan", "QUALITY.USER");

        Assert.Equal(ControlPlanStatus.Draft, plan.Status);
        Assert.True(plan.IsEditable);
        Assert.Equal("quality.user", plan.CreatedBy);
    }

    [Fact]
    public void SubmitAndApprove_TransitionsWorkflow()
    {
        var plan = ControlPlan.Create(
            Guid.NewGuid(), "CP-001", "Battery Control Plan", "quality.user");

        plan.SubmitForReview();
        Assert.Equal(ControlPlanStatus.UnderReview, plan.Status);

        plan.Approve("QUALITY.LEADER");
        Assert.Equal(ControlPlanStatus.Approved, plan.Status);
        Assert.Equal("quality.leader", plan.ApprovedBy);
        Assert.False(plan.IsEditable);
    }

    [Fact]
    public void Item_Create_CapturesSharedProcessSnapshot()
    {
        var step = ProductProcessStep.Create(
            Guid.NewGuid(), 20, "Terminal Assembly",
            "Install terminal", "Correct polarity", "quality.user");

        var item = ControlPlanItem.Create(
            Guid.NewGuid(), step, "20.1",
            ControlPlanCharacteristicType.Product,
            "Terminal height", "Press fixture", "SC",
            "12.0 ± 0.2 mm", "Digital height gauge",
            3, "Every hour", "SPC chart",
            "Stop line and segregate product", "quality.user");

        Assert.Equal(step.Id, item.ProductProcessStepId);
        Assert.Equal(20, item.ProcessStepSequenceSnapshot);
        Assert.Equal("Terminal Assembly", item.ProcessStepNameSnapshot);
    }
}
