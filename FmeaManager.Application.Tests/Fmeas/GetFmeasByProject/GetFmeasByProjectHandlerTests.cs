using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.GetFmeasByProject;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Tests.Fmeas.GetFmeasByProject;

public sealed class GetFmeasByProjectHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenProjectExists_ShouldReturnProjectFmeas()
    {
        var project = CreateProject();
        var fmea = Fmea.Create(
            project.Id,
            "PFMEA-001",
            "Packing Process",
            FmeaType.Pfmea,
            "Quality",
            "cris");

        var handler = new GetFmeasByProjectHandler(
            new FakeProjectRepository(project),
            new FakeFmeaRepository(new[] { fmea }));

        var result = await handler.HandleAsync(project.Id);

        var item = Assert.Single(result);
        Assert.Equal(fmea.Id, item.Id);
        Assert.Equal(project.Id, item.ProjectId);
        Assert.Equal("PFMEA-001", item.Number);
    }

    [Fact]
    public async Task HandleAsync_WhenProjectDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new GetFmeasByProjectHandler(
            new FakeProjectRepository(null),
            new FakeFmeaRepository(Array.Empty<Fmea>()));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(Guid.NewGuid()));
    }

    private static Project CreateProject() =>
        Project.Create(
            "FORD-BATT-001",
            "12V Battery Launch",
            "Battery X",
            "Toluca",
            Guid.NewGuid(),
            "Quality",
            "cris");

    private sealed class FakeProjectRepository : IProjectRepository
    {
        private readonly Project? _project;

        public FakeProjectRepository(Project? project) => _project = project;

        public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_project is not null && _project.Id == id ? _project : null);

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>(Array.Empty<Project>());

        public void Add(Project project) => throw new NotSupportedException();
    }

    private sealed class FakeFmeaRepository : IFmeaRepository
    {
        private readonly IReadOnlyList<Fmea> _fmeas;

        public FakeFmeaRepository(IReadOnlyList<Fmea> fmeas) => _fmeas = fmeas;

        public Task<bool> ExistsByNumberAsync(Guid projectId, string number, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Fmea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_fmeas.SingleOrDefault(fmea => fmea.Id == id));

        public Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Fmea>>(_fmeas.Where(fmea => fmea.ProjectId == projectId).ToList());

        public void Add(Fmea fmea) => throw new NotSupportedException();
    }
}
