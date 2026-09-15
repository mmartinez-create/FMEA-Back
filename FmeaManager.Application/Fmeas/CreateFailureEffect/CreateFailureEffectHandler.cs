using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateFailureEffect;

public sealed class CreateFailureEffectHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IFailureEffectRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFailureEffectHandler(
        FmeaEditabilityGuard editabilityGuard,
        IFailureEffectRepository failureEffectRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _repository = failureEffectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFailureEffectResult> HandleAsync(
        CreateFailureEffectCommand command,
        CancellationToken cancellationToken = default)
    {
        var failureMode = await _editabilityGuard.EnsureFailureModeEditableAsync(command.FailureModeId, cancellationToken);
        var entity = FailureEffect.Create(failureMode.Id, command.Description);

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateFailureEffectResult(
            entity.Id,
            entity.FailureModeId,
            entity.Description);
    }
}
