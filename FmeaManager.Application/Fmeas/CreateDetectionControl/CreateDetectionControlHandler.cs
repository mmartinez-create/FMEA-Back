using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateDetectionControl;

public sealed class CreateDetectionControlHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IDetectionControlRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDetectionControlHandler(
        FmeaEditabilityGuard editabilityGuard,
        IDetectionControlRepository detectionControlRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _repository = detectionControlRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateDetectionControlResult> HandleAsync(
        CreateDetectionControlCommand command,
        CancellationToken cancellationToken = default)
    {
        var failureCause = await _editabilityGuard.EnsureFailureCauseEditableAsync(command.FailureCauseId, cancellationToken);
        var entity = DetectionControl.Create(failureCause.Id, command.Description);

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateDetectionControlResult(
            entity.Id,
            entity.FailureCauseId,
            entity.Description);
    }
}
