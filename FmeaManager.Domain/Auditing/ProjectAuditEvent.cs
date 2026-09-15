namespace FmeaManager.Domain.Auditing;

public sealed class ProjectAuditEvent
{
    private ProjectAuditEvent()
    {
    }

    private ProjectAuditEvent(
        Guid id,
        Guid projectId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName,
        DateTime occurredAt)
    {
        Id = id;
        ProjectId = projectId;
        EventName = eventName;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        ActorUserKey = actorUserKey;
        ActorDisplayName = actorDisplayName;
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public string EventName { get; private set; } = string.Empty;

    public ProjectAuditAction Action { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string ActorUserKey { get; private set; } = string.Empty;

    public string ActorDisplayName { get; private set; } = string.Empty;

    public DateTime OccurredAt { get; private set; }

    public static ProjectAuditEvent Create(
        Guid projectId,
        string eventName,
        ProjectAuditAction action,
        string entityType,
        Guid entityId,
        string actorUserKey,
        string actorDisplayName)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Project is required.",
                nameof(projectId));
        }

        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException(
                "Event name is required.",
                nameof(eventName));
        }

        if (!Enum.IsDefined(action))
        {
            throw new ArgumentOutOfRangeException(
                nameof(action),
                action,
                "Audit action is invalid.");
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException(
                "Entity type is required.",
                nameof(entityType));
        }

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException(
                "Entity id is required.",
                nameof(entityId));
        }

        if (string.IsNullOrWhiteSpace(actorUserKey))
        {
            throw new ArgumentException(
                "Actor user key is required.",
                nameof(actorUserKey));
        }

        var displayName = string.IsNullOrWhiteSpace(actorDisplayName)
            ? actorUserKey.Trim()
            : actorDisplayName.Trim();

        return new ProjectAuditEvent(
            Guid.NewGuid(),
            projectId,
            eventName.Trim(),
            action,
            entityType.Trim(),
            entityId,
            actorUserKey.Trim().ToLowerInvariant(),
            displayName,
            DateTime.UtcNow);
    }
}
