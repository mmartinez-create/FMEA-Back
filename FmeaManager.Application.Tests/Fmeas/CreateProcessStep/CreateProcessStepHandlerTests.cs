using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.CreateProcessStep;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.CreateProcessStep;

public sealed class CreateProcessStepHandlerTests
{
    [Fact]
    public async Task HandleAsync_OnDraftRevision_ShouldPersistProcessStep()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var processStepRepository = new FakeProcessStepRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateProcessStepHandler(
            new FakeRevisionRepository(revision),
            processStepRepository,
            unitOfWork);

        var result = await handler.HandleAsync(new CreateProcessStepCommand(
            revision.Id,
            10,
            " Receiving ",
            " Receive material ",
            " Inspect incoming material "));

        Assert.NotNull(processStepRepository.AddedProcessStep);
        Assert.Equal(10, result.Sequence);
        Assert.Equal("Receiving", result.Name);
        Assert.Equal("Receive material", result.Function);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenSequenceAlreadyExists_ShouldThrowConflictException()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");

        var handler = new CreateProcessStepHandler(
            new FakeRevisionRepository(revision),
            new FakeProcessStepRepository(sequenceExists: true),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateProcessStepCommand(
                revision.Id,
                10,
                "Receiving",
                null,
                null)));

        Assert.Contains("sequence", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenRevisionIsUnderReview_ShouldThrowConflictException()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();

        var handler = new CreateProcessStepHandler(
            new FakeRevisionRepository(revision),
            new FakeProcessStepRepository(),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateProcessStepCommand(
                revision.Id,
                10,
                "Receiving",
                null,
                null)));

        Assert.Contains("not editable", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly FmeaRevision? _revision;

        public FakeRevisionRepository(FmeaRevision? revision) => _revision = revision;

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_revision is not null && _revision.Id == id ? _revision : null);

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(
                _revision is null ? Array.Empty<FmeaRevision>() : new[] { _revision });

        public void Add(FmeaRevision revision) => throw new NotSupportedException();
    }

    private sealed class FakeProcessStepRepository : IProcessStepRepository
    {
        private readonly bool _sequenceExists;

        public FakeProcessStepRepository(bool sequenceExists = false) =>
            _sequenceExists = sequenceExists;

        public ProcessStep? AddedProcessStep { get; private set; }

        public Task<bool> ExistsBySequenceAsync(Guid fmeaRevisionId, int sequence, CancellationToken cancellationToken = default) =>
            Task.FromResult(_sequenceExists);

        public Task<ProcessStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<ProcessStep?>(null);

        public Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(Guid fmeaRevisionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProcessStep>>(Array.Empty<ProcessStep>());

        public void Add(ProcessStep processStep) => AddedProcessStep = processStep;
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
