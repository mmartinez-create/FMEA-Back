using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateFailureCause;

public sealed class CreateFailureCauseHandler
{
    private readonly FmeaEditabilityGuard _editabilityGuard;
    private readonly IFailureCauseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFailureCauseHandler(
        FmeaEditabilityGuard editabilityGuard,
        IFailureCauseRepository failureCauseRepository,
        IUnitOfWork unitOfWork)
    {
        _editabilityGuard = editabilityGuard;
        _repository = failureCauseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFailureCauseResult> HandleAsync(
        CreateFailureCauseCommand command,
        CancellationToken cancellationToken = default)
    {
        var failureMode = await _editabilityGuard.EnsureFailureModeEditableAsync(command.FailureModeId, cancellationToken);
        var entity = FailureCause.Create(failureMode.Id, command.Description);

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateFailureCauseResult(
            entity.Id,
            entity.FailureModeId,
            entity.Description);
    }
}
