using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.ProductStructure.CreatePlant;

public sealed class CreatePlantHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlantHandler(
        IPlantRepository plantRepository,
        IUnitOfWork unitOfWork)
    {
        _plantRepository = plantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePlantResult> HandleAsync(
        CreatePlantCommand command,
        CancellationToken cancellationToken = default)
    {
        var plant = Plant.Create(command.Code, command.Name);

        if (await _plantRepository.ExistsByCodeAsync(
                plant.Code,
                cancellationToken))
        {
            throw new ConflictException(
                $"A plant with code '{plant.Code}' already exists.");
        }

        _plantRepository.Add(plant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePlantResult(
            plant.Id,
            plant.Code,
            plant.Name,
            plant.IsActive);
    }
}
