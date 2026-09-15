using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Projects.GetProjectById;

public sealed class GetProjectByIdHandler
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<GetProjectByIdResult> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException($"Project '{id}' was not found.");
        }

        return new GetProjectByIdResult(
            project.Id,
            project.Code,
            project.Name,
            project.Product,
            project.Plant,
            project.CustomerProfileId,
            project.Owner,
            project.Status,
            project.CreatedAt,
            project.CreatedBy,
            project.UpdatedAt);
    }
}
