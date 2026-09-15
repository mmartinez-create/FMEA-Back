using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreatePreventionControl;

public sealed class CreatePreventionControlHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IPreventionControlRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePreventionControlHandler(
        FmeaEditabilityGuard editabilityGuard,
        IPreventionControlRepository preventionControlRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _repository = preventionControlRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePreventionControlResult> HandleAsync(
        CreatePreventionControlCommand command,
        CancellationToken cancellationToken = default)
    {
        var failureCause = await _editabilityGuard.EnsureFailureCauseEditableAsync(command.FailureCauseId, cancellationToken);
        var entity = PreventionControl.Create(failureCause.Id, command.Description);

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePreventionControlResult(
            entity.Id,
            entity.FailureCauseId,
            entity.Description);
    }
}
