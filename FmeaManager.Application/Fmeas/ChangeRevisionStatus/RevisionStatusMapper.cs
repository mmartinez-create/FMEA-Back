using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.ChangeRevisionStatus;

internal static class RevisionStatusMapper
{
    public static RevisionStatusResult Map(FmeaRevision revision)
    {
        return new RevisionStatusResult(
            revision.Id,
            revision.FmeaId,
            revision.RevisionNumber,
            revision.RevisionCode,
            revision.Status,
            revision.SubmittedAt,
            revision.ApprovedAt,
            revision.ApprovedBy,
            revision.RejectedAt,
            revision.RejectionReason);
    }
}
