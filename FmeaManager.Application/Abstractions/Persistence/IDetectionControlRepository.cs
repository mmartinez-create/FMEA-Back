using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IDetectionControlRepository
{
    Task<DetectionControl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DetectionControl>> ListByFailureCauseIdAsync(Guid failureCauseId, CancellationToken cancellationToken = default);
    void Add(DetectionControl detectionControl);
}
