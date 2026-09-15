using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.ChangeRevisionStatus;

public sealed class ApproveRevisionHandler
{
    private readonly IFmeaRevisionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveRevisionHandler(
        IFmeaRevisionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RevisionStatusResult> HandleAsync(
        Guid revisionId,
        string approvedBy,
        CancellationToken cancellationToken = default)
    {
        var revision = await _repository.GetByIdAsync(
            revisionId,
            cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{revisionId}' was not found.");
        }

        revision.Approve(approvedBy);

        if (revision.BasedOnRevisionId.HasValue)
        {
            var previousRevision = await _repository.GetByIdAsync(
                revision.BasedOnRevisionId.Value,
                cancellationToken);

            if (previousRevision?.Status == FmeaRevisionStatus.Approved)
            {
                previousRevision.Supersede();
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RevisionStatusMapper.Map(revision);
    }
}
