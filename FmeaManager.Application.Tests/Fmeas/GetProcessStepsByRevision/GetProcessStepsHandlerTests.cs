using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.GetProcessStepsByRevision;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.GetProcessStepsByRevision;

public sealed class GetProcessStepsHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenRevisionExists_ShouldReturnOrderedRepositoryResult()
    {
        var fmea = Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-001",
            "Packing",
            FmeaType.Pfmea,
            "Quality",
            "cris");
        var revision = FmeaRevision.CreateInitial(fmea.Id, "cris");
        var step = ProcessStep.Create(
            revision.Id,
            10,
            "Incoming Material",
            "Receive material",
            "Material conforms to specification");

        var handler = new GetProcessStepsHandler(
            new FakeRevisionRepository(revision),
            new FakeProcessStepRepository(new[] { step }));

        var result = await handler.HandleAsync(revision.Id);

        var item = Assert.Single(result);
        Assert.Equal(step.Id, item.Id);
        Assert.Equal(10, item.Sequence);
        Assert.Equal("Incoming Material", item.Name);
    }

    [Fact]
    public async Task HandleAsync_WhenRevisionDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new GetProcessStepsHandler(
            new FakeRevisionRepository(null),
            new FakeProcessStepRepository(Array.Empty<ProcessStep>()));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(Guid.NewGuid()));
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly FmeaRevision? _revision;

        public FakeRevisionRepository(FmeaRevision? revision) => _revision = revision;

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_revision is not null && _revision.Id == id ? _revision : null);

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(Array.Empty<FmeaRevision>());

        public void Add(FmeaRevision revision) => throw new NotSupportedException();
    }

    private sealed class FakeProcessStepRepository : IProcessStepRepository
    {
        private readonly IReadOnlyList<ProcessStep> _steps;

        public FakeProcessStepRepository(IReadOnlyList<ProcessStep> steps) => _steps = steps;

        public Task<bool> ExistsBySequenceAsync(Guid fmeaRevisionId, int sequence, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<ProcessStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_steps.SingleOrDefault(step => step.Id == id));

        public Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(Guid revisionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProcessStep>>(_steps.Where(step => step.FmeaRevisionId == revisionId).OrderBy(step => step.Sequence).ToList());

        public void Add(ProcessStep processStep) => throw new NotSupportedException();
    }
}
