using FmeaManager.Domain.Auditing;

namespace FmeaManager.Domain.Tests.Auditing;

public sealed class ProjectAuditEventTests
{
    [Fact]
    public void Create_WithValidData_NormalizesActorAndKeepsEventData()
    {
        var projectId = Guid.NewGuid();
        var entityId = Guid.NewGuid();

        var auditEvent = ProjectAuditEvent.Create(
            projectId,
            " PFMEA edited ",
            ProjectAuditAction.Edited,
            " Fmea ",
            entityId,
            " QUALITY.USER@COMPANY.COM ",
            "Quality User");

        Assert.Equal(projectId, auditEvent.ProjectId);
        Assert.Equal("PFMEA edited", auditEvent.EventName);
        Assert.Equal(ProjectAuditAction.Edited, auditEvent.Action);
        Assert.Equal("Fmea", auditEvent.EntityType);
        Assert.Equal(entityId, auditEvent.EntityId);
        Assert.Equal("quality.user@company.com", auditEvent.ActorUserKey);
        Assert.Equal("Quality User", auditEvent.ActorDisplayName);
    }

    [Fact]
    public void Create_WithoutDisplayName_UsesUserKey()
    {
        var auditEvent = ProjectAuditEvent.Create(
            Guid.NewGuid(),
            "Process step added",
            ProjectAuditAction.Edited,
            "ProcessStep",
            Guid.NewGuid(),
            "quality.user",
            "");

        Assert.Equal("quality.user", auditEvent.ActorDisplayName);
    }
}
