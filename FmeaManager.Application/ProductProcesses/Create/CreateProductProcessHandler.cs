using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductProcesses.Common;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.ProductProcesses.Create;

public sealed class CreateProductProcessHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProductProcessRepository _processRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductProcessHandler(
        IProjectRepository projectRepository,
        IProductProcessRepository processRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductProcessWorkspaceResult> HandleAsync(
        CreateProductProcessCommand command,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{command.ProjectId}' was not found.");
        }

        if (!project.ProductId.HasValue)
        {
            throw new ConflictException(
                "The project must be linked to a Product before creating the shared process.");
        }

        var existing = await _processRepository.GetByProjectIdAsync(
            command.ProjectId,
            cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException(
                "This project already has a shared Product Process.");
        }

        var process = ProductProcess.Create(
            project.Id,
            command.Name,
            command.Description,
            command.CreatedBy);

        _processRepository.Add(process);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            Array.Empty<ProductProcessStepResult>());
    }
}
