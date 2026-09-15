using FmeaManager.Application.DocumentTraceability;

namespace FmeaManager.Application.Tests.DocumentTraceability;

public sealed class ProjectDocumentSyncStatusEvaluatorTests
{
    [Fact]
    public void Pfmea_IsMissing_WhenNoPfmeaExists()
    {
        var state = ProjectDocumentSyncStatusEvaluator.EvaluatePfmea(
            sharedStepCount: 3,
            pfmeaDocumentCount: 0,
            linkedSharedStepCount: 0);

        Assert.Equal(
            DocumentSynchronizationState.Missing,
            state);
    }

    [Fact]
    public void Pfmea_NeedsSync_WhenNotAllSharedStepsAreLinked()
    {
        var state = ProjectDocumentSyncStatusEvaluator.EvaluatePfmea(
            sharedStepCount: 3,
            pfmeaDocumentCount: 1,
            linkedSharedStepCount: 2);

        Assert.Equal(
            DocumentSynchronizationState.NeedsSync,
            state);
    }

    [Fact]
    public void Overall_IsSynchronized_WhenBothDownstreamDocumentsAreSynchronized()
    {
        var state = ProjectDocumentSyncStatusEvaluator.EvaluateOverall(
            DocumentSynchronizationState.Ready,
            DocumentSynchronizationState.Synchronized,
            DocumentSynchronizationState.Synchronized);

        Assert.Equal(
            DocumentSynchronizationState.Synchronized,
            state);
    }
}
