using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class ProcessStepSharedSourceTests
{
    [Fact]
    public void CreateLinked_CopiesSharedSnapshotAndStoresLink()
    {
        var productProcessId = Guid.NewGuid();

        var shared = ProductProcessStep.Create(
            productProcessId,
            20,
            "Terminal Assembly",
            "Install terminal",
            "Correct polarity",
            "quality.user");

        var step = ProcessStep.CreateLinked(
            Guid.NewGuid(),
            shared);

        Assert.Equal(shared.Id, step.ProductProcessStepId);
        Assert.Equal(shared.Sequence, step.Sequence);
        Assert.Equal(shared.Name, step.Name);
        Assert.Equal(shared.Function, step.Function);
        Assert.Equal(shared.Requirement, step.Requirement);
    }

    [Fact]
    public void RefreshSnapshotFrom_UsesLatestSharedValues()
    {
        var shared = ProductProcessStep.Create(
            Guid.NewGuid(),
            10,
            "Receiving",
            null,
            null,
            "quality.user");

        var step = ProcessStep.CreateLinked(
            Guid.NewGuid(),
            shared);

        shared.UpdateInformation(
            15,
            "Material Receiving",
            "Receive material",
            "Approved material",
            "quality.user");

        step.RefreshSnapshotFrom(shared);

        Assert.Equal(15, step.Sequence);
        Assert.Equal("Material Receiving", step.Name);
        Assert.Equal("Receive material", step.Function);
        Assert.Equal("Approved material", step.Requirement);
    }
}
