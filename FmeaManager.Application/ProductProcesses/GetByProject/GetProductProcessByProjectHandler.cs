using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductProcesses.Common;

namespace FmeaManager.Application.ProductProcesses.GetByProject;

public sealed class GetProductProcessByProjectHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProductProcessRepository _processRepository;
    private readonly IProductProcessStepRepository _stepRepository;

    public GetProductProcessByProjectHandler(
        IProjectRepository projectRepository,
        IProductProcessRepository processRepository,
        IProductProcessStepRepository stepRepository)
    {
        _projectRepository = projectRepository;
        _processRepository = processRepository;
        _stepRepository = stepRepository;
    }

    public async Task<ProductProcessWorkspaceResult?> HandleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(
            projectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{projectId}' was not found.");
        }

        var process = await _processRepository.GetByProjectIdAsync(
            projectId,
            cancellationToken);

        if (process is null)
        {
            return null;
        }

        var steps = await _stepRepository.ListByProductProcessIdAsync(
            process.Id,
            cancellationToken);

        return new ProductProcessWorkspaceResult(
            process.Id,
            process.ProjectId,
            process.Name,
            process.Description,
            process.IsActive,
            process.CreatedAt,
            process.CreatedBy,
            process.UpdatedAt,
            process.UpdatedBy,
            steps
                .OrderBy(step => step.Sequence)
                .Select(step => new ProductProcessStepResult(
                    step.Id,
                    step.ProductProcessId,
                    process.ProjectId,
                    step.Sequence,
                    step.Name,
                    step.Function,
                    step.Requirement,
                    step.CreatedAt,
                    step.CreatedBy,
                    step.UpdatedAt,
                    step.UpdatedBy))
                .ToList());
    }
}
