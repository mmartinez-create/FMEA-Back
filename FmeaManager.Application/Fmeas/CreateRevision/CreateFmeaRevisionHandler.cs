using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateRevision;

public sealed class CreateFmeaRevisionHandler
{
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IFmeaRevisionContentCloner _contentCloner;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFmeaRevisionHandler(
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository,
        IFmeaRevisionContentCloner contentCloner,
        IUnitOfWork unitOfWork)
    {
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
        _contentCloner = contentCloner;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFmeaRevisionResult> HandleAsync(
        CreateFmeaRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var fmea = await _fmeaRepository.GetByIdAsync(
            command.FmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{command.FmeaId}' was not found.");
        }

        if (!fmea.IsActive)
        {
            throw new ConflictException(
                $"FMEA '{fmea.Number}' is inactive.");
        }

        var previousRevision = await _revisionRepository.GetByIdAsync(
            command.BasedOnRevisionId,
            cancellationToken);

        if (previousRevision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{command.BasedOnRevisionId}' was not found.");
        }

        if (previousRevision.FmeaId != fmea.Id)
        {
            throw new ConflictException(
                "The selected base revision does not belong to the requested FMEA.");
        }

        var revisions = await _revisionRepository.ListByFmeaIdAsync(
            fmea.Id,
            cancellationToken);

        var openRevision = revisions.FirstOrDefault(
            revision => revision.Status is
                FmeaRevisionStatus.Draft or
                FmeaRevisionStatus.UnderReview or
                FmeaRevisionStatus.Rejected);

        if (openRevision is not null)
        {
            throw new ConflictException(
                $"Revision '{openRevision.RevisionCode}' is already open. Complete it before creating another revision.");
        }

        var latestApproved = revisions
            .Where(revision => revision.Status == FmeaRevisionStatus.Approved)
            .OrderByDescending(revision => revision.RevisionNumber)
            .FirstOrDefault();

        if (latestApproved is null || latestApproved.Id != previousRevision.Id)
        {
            throw new ConflictException(
                "A new revision must be based on the latest approved revision.");
        }

        var revision = FmeaRevision.CreateNextFrom(
            previousRevision,
            command.CreatedBy,
            command.RevisionReason);

        _revisionRepository.Add(revision);

        await _contentCloner.CloneAsync(
            previousRevision.Id,
            revision.Id,
            command.CreatedBy,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateFmeaRevisionResult(
            revision.Id,
            revision.FmeaId,
            revision.RevisionNumber,
            revision.RevisionCode,
            revision.BasedOnRevisionId,
            revision.RevisionReason,
            revision.Status,
            revision.CreatedAt,
            revision.CreatedBy);
    }
}
