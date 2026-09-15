using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductProcesses.Common;

namespace FmeaManager.Application.ProductProcesses.UpdateStep;

public sealed class UpdateProductProcessStepHandler
{
    private readonly IProductProcessRepository _processRepository;
    private readonly IProductProcessStepRepository _stepRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductProcessStepHandler(
        IProductProcessRepository processRepository,
        IProductProcessStepRepository stepRepository,
        IUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _stepRepository = stepRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductProcessStepResult> HandleAsync(
        UpdateProductProcessStepCommand command,
        CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetByIdAsync(
            command.StepId,
            cancellationToken);

        if (step is null)
        {
            throw new NotFoundException(
                $"Product process step '{command.StepId}' was not found.");
        }

        var process = await _processRepository.GetByIdAsync(
            step.ProductProcessId,
            cancellationToken);

        if (process is null)
        {
            throw new NotFoundException(
                $"Product process '{step.ProductProcessId}' was not found.");
        }

        if (await _stepRepository.ExistsBySequenceAsync(
                process.Id,
                command.Sequence,
                step.Id,
                cancellationToken))
        {
            throw new ConflictException(
                $"Sequence '{command.Sequence}' already exists in this Product Process.");
        }

        step.UpdateInformation(
            command.Sequence,
            command.Name,
            command.Function,
            command.Requirement,
            command.UpdatedBy);

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
