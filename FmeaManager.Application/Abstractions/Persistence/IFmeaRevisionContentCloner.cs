namespace FmeaManager.Application.Abstractions.Persistence;

public interface IFmeaRevisionContentCloner
{
    Task CloneAsync(
        Guid sourceRevisionId,
        Guid targetRevisionId,
        string createdBy,
        CancellationToken cancellationToken = default);
}
