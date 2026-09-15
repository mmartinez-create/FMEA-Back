using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.GetRevisionsByFmea;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.GetRevisionsByFmea;

public sealed class GetFmeaRevisionsHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenFmeaExists_ShouldReturnRevisions()
    {
        var fmea = Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-001",
            "Packing",
            FmeaType.Pfmea,
            "Quality",
            "cris");
        var revision = FmeaRevision.CreateInitial(fmea.Id, "cris");

        var handler = new GetFmeaRevisionsHandler(
            new FakeFmeaRepository(fmea),
            new FakeRevisionRepository(new[] { revision }));

        var result = await handler.HandleAsync(fmea.Id);

        var item = Assert.Single(result);
        Assert.Equal(revision.Id, item.Id);
        Assert.Equal("01", item.RevisionCode);
        Assert.Equal(FmeaRevisionStatus.Draft, item.Status);
    }

    [Fact]
    public async Task HandleAsync_WhenFmeaDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new GetFmeaRevisionsHandler(
            new FakeFmeaRepository(null),
            new FakeRevisionRepository(Array.Empty<FmeaRevision>()));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(Guid.NewGuid()));
    }

    private sealed class FakeFmeaRepository : IFmeaRepository
    {
        private readonly Fmea? _fmea;

        public FakeFmeaRepository(Fmea? fmea) => _fmea = fmea;

        public Task<bool> ExistsByNumberAsync(Guid projectId, string number, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Fmea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_fmea is not null && _fmea.Id == id ? _fmea : null);

        public Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Fmea>>(Array.Empty<Fmea>());

        public void Add(Fmea fmea) => throw new NotSupportedException();
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly IReadOnlyList<FmeaRevision> _revisions;

        public FakeRevisionRepository(IReadOnlyList<FmeaRevision> revisions) => _revisions = revisions;

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_revisions.SingleOrDefault(revision => revision.Id == id));

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(_revisions.Where(revision => revision.FmeaId == fmeaId).ToList());

        public void Add(FmeaRevision revision) => throw new NotSupportedException();
    }
}
