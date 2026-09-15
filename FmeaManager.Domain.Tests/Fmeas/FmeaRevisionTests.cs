using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Domain.Tests.Fmeas;

public sealed class FmeaRevisionTests
{
    [Fact]
    public void CreateInitial_ShouldCreateRevision01InDraftStatus()
    {
        var fmeaId = Guid.NewGuid();

        var revision = FmeaRevision.CreateInitial(fmeaId, "cris");

        Assert.NotEqual(Guid.Empty, revision.Id);
        Assert.Equal(fmeaId, revision.FmeaId);
        Assert.Equal(1, revision.RevisionNumber);
        Assert.Equal("01", revision.RevisionCode);
        Assert.Null(revision.BasedOnRevisionId);
        Assert.Equal("Initial revision", revision.RevisionReason);
        Assert.Equal(FmeaRevisionStatus.Draft, revision.Status);
        Assert.Equal("cris", revision.CreatedBy);
    }

    [Fact]
    public void Workflow_ShouldMoveFromDraftToApproved()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");

        revision.SubmitForReview();
        Assert.Equal(FmeaRevisionStatus.UnderReview, revision.Status);
        Assert.NotNull(revision.SubmittedAt);

        revision.Approve("quality.manager");
        Assert.Equal(FmeaRevisionStatus.Approved, revision.Status);
        Assert.Equal("quality.manager", revision.ApprovedBy);
        Assert.NotNull(revision.ApprovedAt);
    }

    [Fact]
    public void Reject_WithoutReason_ShouldThrowArgumentException()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();

        var action = () => revision.Reject("   ");

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("reason", exception.ParamName);
    }

    [Fact]
    public void RejectedRevision_CanReturnToDraft()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();
        revision.Reject("Missing process information");

        Assert.Equal(FmeaRevisionStatus.Rejected, revision.Status);
        Assert.Equal("Missing process information", revision.RejectionReason);

        revision.ReturnToDraft();

        Assert.Equal(FmeaRevisionStatus.Draft, revision.Status);
    }

    [Fact]
    public void CreateNextFrom_ApprovedRevision_ShouldCreateNextDraftRevision()
    {
        var previous = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        previous.SubmitForReview();
        previous.Approve("quality.manager");

        var next = FmeaRevision.CreateNextFrom(
            previous,
            "cris",
            "Process update");

        Assert.Equal(previous.FmeaId, next.FmeaId);
        Assert.Equal(2, next.RevisionNumber);
        Assert.Equal("02", next.RevisionCode);
        Assert.Equal(previous.Id, next.BasedOnRevisionId);
        Assert.Equal("Process update", next.RevisionReason);
        Assert.Equal(FmeaRevisionStatus.Draft, next.Status);
    }

    [Fact]
    public void CreateNextFrom_DraftRevision_ShouldThrowInvalidOperationException()
    {
        var previous = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");

        var action = () => FmeaRevision.CreateNextFrom(
            previous,
            "cris",
            "Process update");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void UpdateRevisionReason_WhenRevisionIsApproved_ShouldThrowInvalidOperationException()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();
        revision.Approve("quality.manager");

        var action = () => revision.UpdateRevisionReason("Changed reason");

        Assert.Throws<InvalidOperationException>(action);
    }
}
