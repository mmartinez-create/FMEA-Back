using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductProcesses.Common;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.ProductProcesses.CreateStep;

public sealed class CreateProductProcessStepHandler
{
    private readonly IProductProcessRepository _processRepository;
    private readonly IProductProcessStepRepository _stepRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductProcessStepHandler(
        IProductProcessRepository processRepository,
        IProductProcessStepRepository stepRepository,
        IUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _stepRepository = stepRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductProcessStepResult> HandleAsync(
        CreateProductProcessStepCommand command,
        CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetByIdAsync(
            command.ProductProcessId,
            cancellationToken);

        if (process is null)
        {
            throw new NotFoundException(
                $"Product process '{command.ProductProcessId}' was not found.");
        }

        if (!process.IsActive)
        {
            throw new ConflictException(
                "The Product Process is inactive.");
        }

        if (await _stepRepository.ExistsBySequenceAsync(
                process.Id,
                command.Sequence,
                null,
                cancellationToken))
        {
            throw new ConflictException(
                $"Sequence '{command.Sequence}' already exists in this Product Process.");
        }

        var step = ProductProcessStep.Create(
            process.Id,
            command.Sequence,
            command.Name,
            command.Function,
            command.Requirement,
            command.CreatedBy);

        _stepRepository.Add(step);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductProcessStepResult(
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
            step.UpdatedBy);
    }
}
