using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> ListAsync(
        CancellationToken cancellationToken = default);

    void Add(Project project);
}
