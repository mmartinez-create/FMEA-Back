using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetPreventionControlsByFailureCause;

public sealed class GetPreventionControlsByFailureCauseHandler
{
    private readonly IFailureCauseRepository _parentRepository;
    private readonly IPreventionControlRepository _repository;

    public GetPreventionControlsByFailureCauseHandler(
        IFailureCauseRepository parentRepository,
        IPreventionControlRepository repository)
    {
        _parentRepository = parentRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<PreventionControlSummary>> HandleAsync(
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        var parent = await _parentRepository.GetByIdAsync(
            failureCauseId,
            cancellationToken);

        if (parent is null)
        {
            throw new NotFoundException(
                $"Failure cause '{failureCauseId}' was not found.");
        }

        var controls = await _repository.ListByFailureCauseIdAsync(
            failureCauseId,
            cancellationToken);

        return controls
            .Select(control => new PreventionControlSummary(
                control.Id,
        control.FailureCauseId,
        control.Description))
            .ToList();
    }
}
