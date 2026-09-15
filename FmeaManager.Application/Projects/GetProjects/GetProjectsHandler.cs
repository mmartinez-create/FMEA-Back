using FmeaManager.Application.Abstractions.Persistence;

namespace FmeaManager.Application.Projects.GetProjects;

public sealed class GetProjectsHandler
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<ProjectSummary>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.ListAsync(cancellationToken);

        return projects
            .Select(project => new ProjectSummary(
                project.Id,
                project.Code,
                project.Name,
                project.Product,
                project.Plant,
                project.CustomerProfileId,
                project.Owner,
                project.Status,
                project.CreatedAt,
                project.UpdatedAt))
            .ToList();
    }
}
