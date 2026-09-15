using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.ChangeRevisionStatus;

public sealed class RejectRevisionHandler
{
    private readonly IFmeaRevisionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectRevisionHandler(
        IFmeaRevisionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RevisionStatusResult> HandleAsync(
        Guid revisionId,
        string reason,
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

        revision.Reject(reason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RevisionStatusMapper.Map(revision);
    }
}
