using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Projects.CreateProject;

public sealed class CreateProjectHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectHandler(
        IProjectRepository projectRepository,
        ICustomerProfileRepository customerProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _customerProfileRepository = customerProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProjectResult> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var project = Project.Create(
            command.Code,
            command.Name,
            command.Product,
            command.Plant,
            command.CustomerProfileId,
            command.Owner,
            command.CreatedBy);

        var customerProfile = await _customerProfileRepository.GetByIdAsync(
            project.CustomerProfileId,
            cancellationToken);

        if (customerProfile is null)
        {
            throw new NotFoundException(
                $"Customer profile '{project.CustomerProfileId}' was not found.");
        }

        if (!customerProfile.IsActive)
        {
            throw new ConflictException(
                $"Customer profile '{customerProfile.Code}' is inactive.");
        }

        var codeExists = await _projectRepository.ExistsByCodeAsync(
            project.Code,
            cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                $"A project with code '{project.Code}' already exists.");
        }

        _projectRepository.Add(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProjectResult(
            project.Id,
            project.Code,
            project.Name,
            project.Product,
            project.Plant,
            project.CustomerProfileId,
            project.Owner,
            project.Status,
            project.CreatedAt,
            project.CreatedBy);
    }
}
