using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Auditing;
using FmeaManager.Domain.Auditing;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Tests.Auditing;

public sealed class GetLastProjectActivityHandlerTests
{
    [Fact]
    public async Task Handle_WhenAuditExists_ReturnsLatestEvent()
    {
        var project = Project.Create(
            "PROJ-01",
            "Project 01",
            "Battery",
            "Toluca",
            Guid.NewGuid(),
            "Quality",
            "creator");

        var auditEvent = ProjectAuditEvent.Create(
            project.Id,
            "PFMEA edited",
            ProjectAuditAction.Edited,
            "Fmea",
            Guid.NewGuid(),
            "quality.user",
            "Quality User");

        var handler = new GetLastProjectActivityHandler(
            new FakeProjectRepository(project),
            new FakeAuditRepository(auditEvent));

        var result = await handler.HandleAsync(project.Id);

        Assert.NotNull(result);
        Assert.Equal("PFMEA edited", result!.EventName);
        Assert.Equal("quality.user", result.ActorUserKey);
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
            Task.FromResult<IReadOnlyList<Project>>(new[] { _project });

        public void Add(Project project) =>
            throw new NotSupportedException();
    }

    private sealed class FakeAuditRepository : IProjectAuditRepository
    {
        private readonly ProjectAuditEvent _auditEvent;

        public FakeAuditRepository(ProjectAuditEvent auditEvent)
        {
            _auditEvent = auditEvent;
        }

        public Task<ProjectAuditEvent?> GetLastByProjectIdAsync(
            Guid projectId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProjectAuditEvent?>(
                projectId == _auditEvent.ProjectId
                    ? _auditEvent
                    : null);

        public Task<IReadOnlyList<ProjectAuditEvent>> ListByProjectIdAsync(
            Guid projectId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProjectAuditEvent>>(
                projectId == _auditEvent.ProjectId
                    ? new[] { _auditEvent }
                    : Array.Empty<ProjectAuditEvent>());

        public void Add(ProjectAuditEvent auditEvent) =>
            throw new NotSupportedException();
    }
}
