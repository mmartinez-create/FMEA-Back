using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateFailureMode;

public sealed class CreateFailureModeHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IFailureModeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFailureModeHandler(
        FmeaEditabilityGuard editabilityGuard,
        IFailureModeRepository failureModeRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _repository = failureModeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFailureModeResult> HandleAsync(
        CreateFailureModeCommand command,
        CancellationToken cancellationToken = default)
    {
        var processStep = await _editabilityGuard.EnsureProcessStepEditableAsync(command.ProcessStepId, cancellationToken);
        var entity = FailureMode.Create(processStep.Id, command.Description);

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateFailureModeResult(
            entity.Id,
            entity.ProcessStepId,
            entity.Description);
    }
}
