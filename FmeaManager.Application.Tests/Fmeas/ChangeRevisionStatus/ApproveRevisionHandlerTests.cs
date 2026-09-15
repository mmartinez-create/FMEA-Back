using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.ChangeRevisionStatus;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.ChangeRevisionStatus;

public sealed class ApproveRevisionHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenApprovingNextRevision_ShouldSupersedePreviousApprovedRevision()
    {
        var fmeaId = Guid.NewGuid();
        var previous = FmeaRevision.CreateInitial(fmeaId, "author");
        previous.SubmitForReview();
        previous.Approve("approver");

        var next = FmeaRevision.CreateNextFrom(previous, "author", "Process update");
        next.SubmitForReview();

        var repository = new FakeRevisionRepository(previous, next);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new ApproveRevisionHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(next.Id, "quality-manager");

        Assert.Equal(FmeaRevisionStatus.Approved, result.Status);
        Assert.Equal(FmeaRevisionStatus.Superseded, previous.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly List<FmeaRevision> _revisions;

        public FakeRevisionRepository(params FmeaRevision[] revisions)
        {
            _revisions = revisions.ToList();
        }

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_revisions.SingleOrDefault(revision => revision.Id == id));

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(
                _revisions.Where(revision => revision.FmeaId == fmeaId).ToList());

        public void Add(FmeaRevision revision) => _revisions.Add(revision);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
