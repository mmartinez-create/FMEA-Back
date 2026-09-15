using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Projects.CreateProject;
using FmeaManager.Domain.CustomerProfiles;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Tests.Projects.CreateProject;

public sealed class CreateProjectHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldPersistProject()
    {
        var customerProfile = CustomerProfile.Create(
            "FORD",
            "Ford",
            "Ford profile");

        var projectRepository = new FakeProjectRepository();
        var customerProfileRepository =
            new FakeCustomerProfileRepository(customerProfile);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateProjectHandler(
            projectRepository,
            customerProfileRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new CreateProjectCommand(
                "FORD-BATT-001",
                "12V Battery Launch",
                "Battery X",
                "Toluca",
                customerProfile.Id,
                "Quality Engineering",
                "cris"));

        Assert.NotNull(projectRepository.AddedProject);
        Assert.Equal("FORD-BATT-001", result.Code);
        Assert.Equal(ProjectStatus.Draft, result.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerProfileDoesNotExist_ShouldThrowNotFoundException()
    {
        var handler = new CreateProjectHandler(
            new FakeProjectRepository(),
            new FakeCustomerProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(
                new CreateProjectCommand(
                    "FORD-BATT-001",
                    "12V Battery Launch",
                    "Battery X",
                    "Toluca",
                    Guid.NewGuid(),
                    "Quality Engineering",
                    "cris")));
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerProfileIsInactive_ShouldThrowConflictException()
    {
        var customerProfile = CustomerProfile.Create(
            "FORD",
            "Ford",
            "Ford profile");

        customerProfile.Deactivate();

        var handler = new CreateProjectHandler(
            new FakeProjectRepository(),
            new FakeCustomerProfileRepository(customerProfile),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new CreateProjectCommand(
                    "FORD-BATT-001",
                    "12V Battery Launch",
                    "Battery X",
                    "Toluca",
                    customerProfile.Id,
                    "Quality Engineering",
                    "cris")));
    }

    [Fact]
    public async Task HandleAsync_WhenProjectCodeAlreadyExists_ShouldThrowConflictException()
    {
        var customerProfile = CustomerProfile.Create(
            "FORD",
            "Ford",
            "Ford profile");

        var handler = new CreateProjectHandler(
            new FakeProjectRepository(codeExists: true),
            new FakeCustomerProfileRepository(customerProfile),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new CreateProjectCommand(
                    "FORD-BATT-001",
                    "12V Battery Launch",
                    "Battery X",
                    "Toluca",
                    customerProfile.Id,
                    "Quality Engineering",
                    "cris")));
    }

    [Fact]
    public async Task HandleAsync_WithEmptyCode_ShouldUseDomainValidation()
    {
        var customerProfile = CustomerProfile.Create(
            "FORD",
            "Ford",
            "Ford profile");

        var handler = new CreateProjectHandler(
            new FakeProjectRepository(),
            new FakeCustomerProfileRepository(customerProfile),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(
                new CreateProjectCommand(
                    "",
                    "12V Battery Launch",
                    "Battery X",
                    "Toluca",
                    customerProfile.Id,
                    "Quality Engineering",
                    "cris")));

        Assert.Equal("code", exception.ParamName);
    }

    private sealed class FakeProjectRepository : IProjectRepository
    {
        private readonly bool _codeExists;

        public FakeProjectRepository(bool codeExists = false)
        {
            _codeExists = codeExists;
        }

        public Project? AddedProject { get; private set; }

        public Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_codeExists);
        }

        public Task<Project?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (AddedProject is not null && AddedProject.Id == id)
            {
                return Task.FromResult<Project?>(AddedProject);
            }

            return Task.FromResult<Project?>(null);
        }

        public Task<IReadOnlyList<Project>> ListAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Project> projects = AddedProject is null
                ? Array.Empty<Project>()
                : new[] { AddedProject };

            return Task.FromResult(projects);
        }

        public void Add(Project project)
        {
            AddedProject = project;
        }
    }

    private sealed class FakeCustomerProfileRepository : ICustomerProfileRepository
    {
        private readonly CustomerProfile? _customerProfile;

        public FakeCustomerProfileRepository(
            CustomerProfile? customerProfile)
        {
            _customerProfile = customerProfile;
        }

        public Task<CustomerProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (_customerProfile is not null &&
                _customerProfile.Id == id)
            {
                return Task.FromResult<CustomerProfile?>(_customerProfile);
            }

            return Task.FromResult<CustomerProfile?>(null);
        }

        public Task<IReadOnlyList<CustomerProfile>> ListActiveAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CustomerProfile> profiles =
                _customerProfile is not null &&
                _customerProfile.IsActive
                    ? new[] { _customerProfile }
                    : Array.Empty<CustomerProfile>();

            return Task.FromResult(profiles);
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
