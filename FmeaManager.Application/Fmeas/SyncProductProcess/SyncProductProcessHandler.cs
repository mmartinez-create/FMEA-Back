using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.SyncProductProcess;

public sealed class SyncProductProcessHandler
{
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly PfmeaProcessSynchronizer _synchronizer;
    private readonly IUnitOfWork _unitOfWork;

    public SyncProductProcessHandler(
        IFmeaRevisionRepository revisionRepository,
        PfmeaProcessSynchronizer synchronizer,
        IUnitOfWork unitOfWork)
    {
        _revisionRepository = revisionRepository;
        _synchronizer = synchronizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<SyncProductProcessResult> HandleAsync(
        Guid revisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _revisionRepository.GetByIdAsync(
            revisionId,
            cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{revisionId}' was not found.");
        }

        var result = await _synchronizer.SyncAsync(
            revision,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return result;
    }
}
