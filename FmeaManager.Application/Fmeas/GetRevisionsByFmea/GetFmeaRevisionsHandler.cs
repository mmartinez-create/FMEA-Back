using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetRevisionsByFmea;

public sealed class GetFmeaRevisionsHandler
{
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;

    public GetFmeaRevisionsHandler(
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository)
    {
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
    }

    public async Task<IReadOnlyList<FmeaRevisionSummary>> HandleAsync(
        Guid fmeaId,
        CancellationToken cancellationToken = default)
    {
        var fmea = await _fmeaRepository.GetByIdAsync(
            fmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{fmeaId}' was not found.");
        }

        var revisions = await _revisionRepository.ListByFmeaIdAsync(
            fmeaId,
            cancellationToken);

        return revisions
            .Select(revision => new FmeaRevisionSummary(
                revision.Id,
                revision.FmeaId,
                revision.RevisionNumber,
                revision.RevisionCode,
                revision.BasedOnRevisionId,
                revision.RevisionReason,
                revision.Status,
                revision.CreatedAt,
                revision.CreatedBy,
                revision.SubmittedAt,
                revision.ApprovedAt,
                revision.ApprovedBy,
                revision.RejectedAt,
                revision.RejectionReason))
            .ToList();
    }
}
