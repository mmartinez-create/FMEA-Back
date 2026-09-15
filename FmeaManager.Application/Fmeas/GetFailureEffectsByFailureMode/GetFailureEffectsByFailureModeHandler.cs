using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetFailureEffectsByFailureMode;

public sealed class GetFailureEffectsByFailureModeHandler
{
    private readonly IFailureModeRepository _parentRepository;
    private readonly IFailureEffectRepository _repository;

    public GetFailureEffectsByFailureModeHandler(
        IFailureModeRepository parentRepository,
        IFailureEffectRepository repository)
    {
        _parentRepository = parentRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<FailureEffectSummary>> HandleAsync(
        Guid failureModeId,
        CancellationToken cancellationToken = default)
    {
        var parent = await _parentRepository.GetByIdAsync(
            failureModeId,
            cancellationToken);

        if (parent is null)
        {
            throw new NotFoundException(
                $"Failure mode '{failureModeId}' was not found.");
        }

        var failureEffects = await _repository.ListByFailureModeIdAsync(
            failureModeId,
            cancellationToken);

        return failureEffects
            .Select(effect => new FailureEffectSummary(
                effect.Id,
        effect.FailureModeId,
        effect.Description))
            .ToList();
    }
}
