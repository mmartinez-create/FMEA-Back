using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.GetFmeaById;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.GetFmeaById;

public sealed class GetFmeaByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenFmeaExists_ShouldReturnFmea()
    {
        var fmea = Fmea.Create(
            Guid.NewGuid(),
            "PFMEA-001",
            "Packing Process",
            FmeaType.Pfmea,
            "Quality",
            "cris");

        var handler = new GetFmeaByIdHandler(new FakeFmeaRepository(fmea));

        var result = await handler.HandleAsync(fmea.Id);

        Assert.Equal(fmea.Id, result.Id);
        Assert.Equal("PFMEA-001", result.Number);
        Assert.Equal(FmeaType.Pfmea, result.Type);
    }

    [Fact]
    public async Task HandleAsync_WhenFmeaDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new GetFmeaByIdHandler(new FakeFmeaRepository(null));

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
}
