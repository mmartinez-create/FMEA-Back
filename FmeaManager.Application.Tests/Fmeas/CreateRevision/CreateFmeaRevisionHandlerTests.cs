using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.CreateRevision;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.CreateRevision;

public sealed class CreateFmeaRevisionHandlerTests
{
    [Fact]
    public async Task HandleAsync_FromLatestApprovedRevision_ShouldCreateNextDraftAndCloneContent()
    {
        var fmea = CreateFmea();
        var previous = FmeaRevision.CreateInitial(fmea.Id, "cris");
        previous.SubmitForReview();
        previous.Approve("approver");

        var revisionRepository = new FakeRevisionRepository(previous);
        var contentCloner = new FakeContentCloner();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateFmeaRevisionHandler(
            new FakeFmeaRepository(fmea),
            revisionRepository,
            contentCloner,
            unitOfWork);

        var result = await handler.HandleAsync(new CreateFmeaRevisionCommand(
            fmea.Id,
            previous.Id,
            "Process improvement",
            "cris"));

        Assert.NotNull(revisionRepository.AddedRevision);
        Assert.Equal(2, result.RevisionNumber);
        Assert.Equal("02", result.RevisionCode);
        Assert.Equal(previous.Id, result.BasedOnRevisionId);
        Assert.Equal(FmeaRevisionStatus.Draft, result.Status);
        Assert.Equal(previous.Id, contentCloner.SourceRevisionId);
        Assert.Equal(result.Id, contentCloner.TargetRevisionId);
        Assert.Equal("cris", contentCloner.CreatedBy);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenAnOpenRevisionAlreadyExists_ShouldThrowConflictException()
    {
        var fmea = CreateFmea();
        var approved = FmeaRevision.CreateInitial(fmea.Id, "cris");
        approved.SubmitForReview();
        approved.Approve("approver");

        var open = FmeaRevision.CreateNextFrom(
            approved,
            "cris",
            "Existing change");

        var handler = new CreateFmeaRevisionHandler(
            new FakeFmeaRepository(fmea),
            new FakeRevisionRepository(approved, open),
            new FakeContentCloner(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateFmeaRevisionCommand(
                fmea.Id,
                approved.Id,
                "Another change",
                "cris")));
    }

    [Fact]
    public async Task HandleAsync_WhenBaseRevisionIsNotApproved_ShouldThrowConflictException()
    {
        var fmea = CreateFmea();
        var previous = FmeaRevision.CreateInitial(fmea.Id, "cris");

        var handler = new CreateFmeaRevisionHandler(
            new FakeFmeaRepository(fmea),
            new FakeRevisionRepository(previous),
            new FakeContentCloner(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateFmeaRevisionCommand(
                fmea.Id,
                previous.Id,
                "Change",
                "cris")));
    }

    [Fact]
    public async Task HandleAsync_WhenBaseRevisionBelongsToAnotherFmea_ShouldThrowConflictException()
    {
        var fmea = CreateFmea();
        var otherFmea = Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-999",
            "Other",
            FmeaType.Pfmea,
            "Quality",
            "cris");

        var previous = FmeaRevision.CreateInitial(otherFmea.Id, "cris");
        previous.SubmitForReview();
        previous.Approve("approver");

        var handler = new CreateFmeaRevisionHandler(
            new FakeFmeaRepository(fmea),
            new FakeRevisionRepository(previous),
            new FakeContentCloner(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateFmeaRevisionCommand(
                fmea.Id,
                previous.Id,
                "Change",
                "cris")));
    }

    private static Fmea CreateFmea() =>
        Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-001",
            "Packing",
            FmeaType.Pfmea,
            "Quality",
            "cris");

    private sealed class FakeFmeaRepository : IFmeaRepository
    {
        private readonly Fmea? _fmea;

        public FakeFmeaRepository(Fmea? fmea) => _fmea = fmea;

        public Task<bool> ExistsByNumberAsync(Guid projectId, string number, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Fmea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_fmea is not null && _fmea.Id == id ? _fmea : null);

        public Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Fmea>>(_fmea is null ? Array.Empty<Fmea>() : new[] { _fmea });

        public void Add(Fmea fmea) => throw new NotSupportedException();
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly List<FmeaRevision> _revisions;

        public FakeRevisionRepository(params FmeaRevision[] revisions)
        {
            _revisions = revisions.ToList();
        }

        public FmeaRevision? AddedRevision { get; private set; }

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_revisions.SingleOrDefault(revision => revision.Id == id));

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(
                _revisions
                    .Where(revision => revision.FmeaId == fmeaId)
                    .OrderByDescending(revision => revision.RevisionNumber)
                    .ToList());

        public void Add(FmeaRevision revision)
        {
            AddedRevision = revision;
            _revisions.Add(revision);
        }
    }

    private sealed class FakeContentCloner : IFmeaRevisionContentCloner
    {
        public Guid? SourceRevisionId { get; private set; }
        public Guid? TargetRevisionId { get; private set; }
        public string? CreatedBy { get; private set; }

        public Task CloneAsync(
            Guid sourceRevisionId,
            Guid targetRevisionId,
            string createdBy,
            CancellationToken cancellationToken = default)
        {
            SourceRevisionId = sourceRevisionId;
            TargetRevisionId = targetRevisionId;
            CreatedBy = createdBy;
            return Task.CompletedTask;
        }
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
