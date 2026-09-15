using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetDetectionControlsByFailureCause;

public sealed class GetDetectionControlsByFailureCauseHandler
{
    private readonly IFailureCauseRepository _parentRepository;
    private readonly IDetectionControlRepository _repository;

    public GetDetectionControlsByFailureCauseHandler(
        IFailureCauseRepository parentRepository,
        IDetectionControlRepository repository)
    {
        _parentRepository = parentRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<DetectionControlSummary>> HandleAsync(
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
            .Select(control => new DetectionControlSummary(
                control.Id,
        control.FailureCauseId,
        control.Description))
            .ToList();
    }
}
