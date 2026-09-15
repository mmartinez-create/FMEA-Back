using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductProcesses.Create;
using FmeaManager.Application.ProductProcesses.CreateStep;
using FmeaManager.Domain.ProductStructure;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Tests.ProductProcesses;

public sealed class ProductProcessHandlersTests
{
    [Fact]
    public async Task CreateProcess_ForProductProject_PersistsSharedProcess()
    {
        var project = CreateProductProject();
        var processRepository = new FakeProductProcessRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateProductProcessHandler(
            new FakeProjectRepository(project),
            processRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new CreateProductProcessCommand(
                project.Id,
                "Main Manufacturing Process",
                "Shared source",
                "quality.user"));

        Assert.Equal(project.Id, result.ProjectId);
        Assert.NotNull(processRepository.Added);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateProcess_ForLegacyProjectWithoutProduct_ThrowsConflict()
    {
        var project = Project.Create(
            "LEGACY-01",
            "Legacy",
            "Text product",
            "Text plant",
            Guid.NewGuid(),
            "Quality",
            "creator");

        var handler = new CreateProductProcessHandler(
            new FakeProjectRepository(project),
            new FakeProductProcessRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new CreateProductProcessCommand(
                    project.Id,
                    "Process",
                    null,
                    "quality.user")));
    }

    [Fact]
    public async Task CreateStep_WithUniqueSequence_PersistsStep()
    {
        var project = CreateProductProject();
        var process = ProductProcess.Create(
            project.Id,
            "Main Process",
            null,
            "quality.user");

        var stepRepository = new FakeProductProcessStepRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateProductProcessStepHandler(
            new FakeProductProcessRepository(process),
            stepRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new CreateProductProcessStepCommand(
                process.Id,
                10,
                "Receiving",
                "Receive material",
                "Correct material",
                "quality.user"));

        Assert.Equal(10, result.Sequence);
        Assert.NotNull(stepRepository.Added);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    private static Project CreateProductProject()
    {
        var project = Project.Create(
            "PROJ-01",
            "Project",
            "Battery",
            "Toluca",
            Guid.NewGuid(),
            "Quality",
            "creator");

        project.AssignToProduct(Guid.NewGuid());
        return project;
    }

    private sealed class FakeProjectRepository : IProjectRepository
    {
        private readonly Project _project;

        public FakeProjectRepository(Project project)
        {
            _project = project;
        }

        public Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Project?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(
                id == _project.Id ? _project : null);

        public Task<IReadOnlyList<Project>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>(
                new[] { _project });

        public void Add(Project project) =>
            throw new NotSupportedException();
    }

    private sealed class FakeProductProcessRepository
        : IProductProcessRepository
    {
        private readonly ProductProcess? _existing;

        public FakeProductProcessRepository(
            ProductProcess? existing = null)
        {
            _existing = existing;
        }

        public ProductProcess? Added { get; private set; }

        public Task<ProductProcess?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProductProcess?>(
                _existing is not null && _existing.Id == id
                    ? _existing
                    : Added is not null && Added.Id == id
                        ? Added
                        : null);

        public Task<ProductProcess?> GetByProjectIdAsync(
            Guid projectId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProductProcess?>(
                _existing is not null &&
                _existing.ProjectId == projectId
                    ? _existing
                    : null);

        public void Add(ProductProcess productProcess)
        {
            Added = productProcess;
        }
    }

    private sealed class FakeProductProcessStepRepository
        : IProductProcessStepRepository
    {
        public ProductProcessStep? Added { get; private set; }

        public Task<ProductProcessStep?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProductProcessStep?>(
                Added is not null && Added.Id == id
                    ? Added
                    : null);

        public Task<IReadOnlyList<ProductProcessStep>> ListByProductProcessIdAsync(
            Guid productProcessId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProductProcessStep>>(
                Added is not null &&
                Added.ProductProcessId == productProcessId
                    ? new[] { Added }
                    : Array.Empty<ProductProcessStep>());

        public Task<bool> ExistsBySequenceAsync(
            Guid productProcessId,
            int sequence,
            Guid? excludingStepId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(ProductProcessStep step)
        {
            Added = step;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
