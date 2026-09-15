using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetFailureCausesByFailureMode;

public sealed class GetFailureCausesByFailureModeHandler
{
    private readonly IFailureModeRepository _parentRepository;
    private readonly IFailureCauseRepository _repository;

    public GetFailureCausesByFailureModeHandler(
        IFailureModeRepository parentRepository,
        IFailureCauseRepository repository)
    {
        _parentRepository = parentRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<FailureCauseSummary>> HandleAsync(
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

        var failureCauses = await _repository.ListByFailureModeIdAsync(
            failureModeId,
            cancellationToken);

        return failureCauses
            .Select(cause => new FailureCauseSummary(
                cause.Id,
        cause.FailureModeId,
        cause.Description))
            .ToList();
    }
}
