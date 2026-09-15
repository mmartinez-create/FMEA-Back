using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetFailureModesByProcessStep;

public sealed class GetFailureModesByProcessStepHandler
{
    private readonly IProcessStepRepository _parentRepository;
    private readonly IFailureModeRepository _repository;

    public GetFailureModesByProcessStepHandler(
        IProcessStepRepository parentRepository,
        IFailureModeRepository repository)
    {
        _parentRepository = parentRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<FailureModeSummary>> HandleAsync(
        Guid processStepId,
        CancellationToken cancellationToken = default)
    {
        var parent = await _parentRepository.GetByIdAsync(
            processStepId,
            cancellationToken);

        if (parent is null)
        {
            throw new NotFoundException(
                $"Process step '{processStepId}' was not found.");
        }

        var failureModes = await _repository.ListByProcessStepIdAsync(
            processStepId,
            cancellationToken);

        return failureModes
            .Select(mode => new FailureModeSummary(
                mode.Id,
        mode.ProcessStepId,
        mode.Description))
            .ToList();
    }
}
