using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.SyncProductProcess;

namespace FmeaManager.Application.Fmeas.ChangeRevisionStatus;

public sealed class SubmitRevisionForReviewHandler
{
    private readonly IFmeaRevisionRepository _repository;
    private readonly PfmeaProcessSynchronizer _synchronizer;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitRevisionForReviewHandler(
        IFmeaRevisionRepository repository,
        PfmeaProcessSynchronizer synchronizer,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _synchronizer = synchronizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<RevisionStatusResult> HandleAsync(
        Guid revisionId,
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

        await _synchronizer.SyncAsync(
            revision,
            cancellationToken);

        revision.SubmitForReview();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return RevisionStatusMapper.Map(revision);
    }
}
