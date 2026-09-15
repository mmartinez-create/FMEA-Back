using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.CreateFmea;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Tests.Fmeas.CreateFmea;

public sealed class CreateFmeaHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldPersistFmeaAndInitialDraftRevision()
    {
        var project = CreateProject();
        var fmeaRepository = new FakeFmeaRepository();
        var revisionRepository = new FakeRevisionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateFmeaHandler(
            new FakeProjectRepository(project),
            fmeaRepository,
            revisionRepository,
            unitOfWork);

        var result = await handler.HandleAsync(new CreateFmeaCommand(
            project.Id,
            " pfmea-001 ",
            "Packing Process PFMEA",
            FmeaType.Pfmea,
            "Quality Engineering",
            "cris"));

        Assert.NotNull(fmeaRepository.AddedFmea);
        Assert.NotNull(revisionRepository.AddedRevision);
        Assert.Equal("PFMEA-001", result.Number);
        Assert.Equal("01", result.InitialRevisionCode);
        Assert.Equal(FmeaRevisionStatus.Draft, result.InitialRevisionStatus);
        Assert.Equal(result.Id, revisionRepository.AddedRevision!.FmeaId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenProjectDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new CreateFmeaHandler(
            new FakeProjectRepository(null),
            new FakeFmeaRepository(),
            new FakeRevisionRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(new CreateFmeaCommand(
                Guid.NewGuid(),
                "PFMEA-001",
                "Packing",
                FmeaType.Pfmea,
                "Quality",
                "cris")));
    }

    [Fact]
    public async Task HandleAsync_WhenProjectIsArchived_ShouldThrowConflictException()
    {
        var project = CreateProject();
        project.Activate();
        project.Complete();
        project.Archive();

        var handler = new CreateFmeaHandler(
            new FakeProjectRepository(project),
            new FakeFmeaRepository(),
            new FakeRevisionRepository(),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateFmeaCommand(
                project.Id,
                "PFMEA-001",
                "Packing",
                FmeaType.Pfmea,
                "Quality",
                "cris")));

        Assert.Contains("archived", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenNormalizedNumberAlreadyExists_ShouldThrowConflictException()
    {
        var project = CreateProject();
        var fmeaRepository = new FakeFmeaRepository(numberExists: true);

        var handler = new CreateFmeaHandler(
            new FakeProjectRepository(project),
            fmeaRepository,
            new FakeRevisionRepository(),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateFmeaCommand(
                project.Id,
                " pfmea-001 ",
                "Packing",
                FmeaType.Pfmea,
                "Quality",
                "cris")));

        Assert.Equal("PFMEA-001", fmeaRepository.LastCheckedNumber);
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyNumber_ShouldUseDomainValidation()
    {
        var project = CreateProject();

        var handler = new CreateFmeaHandler(
            new FakeProjectRepository(project),
            new FakeFmeaRepository(),
            new FakeRevisionRepository(),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(new CreateFmeaCommand(
                project.Id,
                "",
                "Packing",
                FmeaType.Pfmea,
                "Quality",
                "cris")));

        Assert.Equal("number", exception.ParamName);
    }

    private static Project CreateProject() =>
        Project.Create(
            "FORD-BATT-001",
            "12V Battery Launch",
            "Battery X",
            "Toluca",
            Guid.NewGuid(),
            "Quality Engineering",
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
            Task.FromResult<IReadOnlyList<Project>>(_project is null ? Array.Empty<Project>() : new[] { _project });

        public void Add(Project project) => throw new NotSupportedException();
    }

    private sealed class FakeFmeaRepository : IFmeaRepository
    {
        private readonly bool _numberExists;

        public FakeFmeaRepository(bool numberExists = false) => _numberExists = numberExists;

        public Fmea? AddedFmea { get; private set; }
        public string? LastCheckedNumber { get; private set; }

        public Task<bool> ExistsByNumberAsync(Guid projectId, string number, CancellationToken cancellationToken = default)
        {
            LastCheckedNumber = number;
            return Task.FromResult(_numberExists);
        }

        public Task<Fmea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Fmea?>(null);

        public Task<IReadOnlyList<Fmea>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Fmea>>(Array.Empty<Fmea>());

        public void Add(Fmea fmea) => AddedFmea = fmea;
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        public FmeaRevision? AddedRevision { get; private set; }

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<FmeaRevision?>(null);

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(Array.Empty<FmeaRevision>());

        public void Add(FmeaRevision revision) => AddedRevision = revision;
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
