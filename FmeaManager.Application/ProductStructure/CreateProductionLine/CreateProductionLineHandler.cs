using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.ProductStructure.CreateProductionLine;

public sealed class CreateProductionLineHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductionLineHandler(
        IPlantRepository plantRepository,
        IProductionLineRepository productionLineRepository,
        IUnitOfWork unitOfWork)
    {
        _plantRepository = plantRepository;
        _productionLineRepository = productionLineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProductionLineResult> HandleAsync(
        CreateProductionLineCommand command,
        CancellationToken cancellationToken = default)
    {
        var plant = await _plantRepository.GetByIdAsync(
            command.PlantId,
            cancellationToken);

        if (plant is null)
        {
            throw new NotFoundException(
                $"Plant '{command.PlantId}' was not found.");
        }

        if (!plant.IsActive)
        {
            throw new ConflictException(
                $"Plant '{plant.Code}' is inactive.");
        }

        var line = ProductionLine.Create(
            plant.Id,
            command.Code,
            command.Name);

        if (await _productionLineRepository.ExistsByCodeAsync(
                plant.Id,
                line.Code,
                cancellationToken))
        {
            throw new ConflictException(
                $"Production line code '{line.Code}' already exists in plant '{plant.Code}'.");
        }

        _productionLineRepository.Add(line);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProductionLineResult(
            line.Id,
            line.PlantId,
            line.Code,
            line.Name,
            line.IsActive);
    }
}
