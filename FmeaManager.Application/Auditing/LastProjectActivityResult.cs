using FmeaManager.Domain.Auditing;

namespace FmeaManager.Application.Auditing;

public sealed record LastProjectActivityResult(
    Guid Id,
    Guid ProjectId,
    string EventName,
    ProjectAuditAction Action,
    string EntityType,
    Guid EntityId,
    string ActorUserKey,
    string ActorDisplayName,
    DateTime OccurredAt);
