using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.ProductStructure.CreateProjectForProduct;

public sealed class CreateProjectForProductHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IPlantRepository _plantRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectForProductHandler(
        IProductRepository productRepository,
        IProductionLineRepository productionLineRepository,
        IPlantRepository plantRepository,
        IProjectRepository projectRepository,
        ICustomerProfileRepository customerProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productionLineRepository = productionLineRepository;
        _plantRepository = plantRepository;
        _projectRepository = projectRepository;
        _customerProfileRepository = customerProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProjectForProductResult> HandleAsync(
        CreateProjectForProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            command.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product '{command.ProductId}' was not found.");
        }

        if (!product.IsActive)
        {
            throw new ConflictException(
                $"Product '{product.Code}' is inactive.");
        }

        var line = await _productionLineRepository.GetByIdAsync(
            product.ProductionLineId,
            cancellationToken);

        if (line is null)
        {
            throw new NotFoundException(
                $"Production line '{product.ProductionLineId}' was not found.");
        }

        var plant = await _plantRepository.GetByIdAsync(
            line.PlantId,
            cancellationToken);

        if (plant is null)
        {
            throw new NotFoundException(
                $"Plant '{line.PlantId}' was not found.");
        }

        var customerProfile = await _customerProfileRepository.GetByIdAsync(
            command.CustomerProfileId,
            cancellationToken);

        if (customerProfile is null)
        {
            throw new NotFoundException(
                $"Customer profile '{command.CustomerProfileId}' was not found.");
        }

        if (!customerProfile.IsActive)
        {
            throw new ConflictException(
                $"Customer profile '{customerProfile.Code}' is inactive.");
        }

        if (await _projectRepository.ExistsByCodeAsync(
                command.Code.Trim().ToUpperInvariant(),
                cancellationToken))
        {
            throw new ConflictException(
                $"A project with code '{command.Code.Trim().ToUpperInvariant()}' already exists.");
        }

        var project = Project.Create(
            command.Code,
            command.Name,
            product.Name,
            plant.Name,
            customerProfile.Id,
            command.Owner,
            command.CreatedBy);

        project.AssignToProduct(product.Id);

        _projectRepository.Add(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProjectForProductResult(
            project.Id,
            product.Id,
            project.Code,
            project.Name,
            product.Name,
            plant.Name,
            project.CustomerProfileId,
            project.Owner,
            project.Status,
            project.CreatedAt);
    }
}
