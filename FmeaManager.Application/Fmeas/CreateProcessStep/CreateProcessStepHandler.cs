using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Fmeas.CreateProcessStep;

public sealed class CreateProcessStepHandler
{
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProcessStepHandler(
        IFmeaRevisionRepository revisionRepository,
        IProcessStepRepository processStepRepository,
        IUnitOfWork unitOfWork)
    {
        _revisionRepository = revisionRepository;
        _processStepRepository = processStepRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProcessStepResult> HandleAsync(
        CreateProcessStepCommand command,
        CancellationToken cancellationToken = default)
    {
        var revision = await _revisionRepository.GetByIdAsync(
            command.FmeaRevisionId,
            cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{command.FmeaRevisionId}' was not found.");
        }

        if (revision.Status is not FmeaRevisionStatus.Draft
            and not FmeaRevisionStatus.Rejected)
        {
            throw new ConflictException(
                $"Revision '{revision.RevisionCode}' is not editable.");
        }

        var sequenceExists = await _processStepRepository.ExistsBySequenceAsync(
            revision.Id,
            command.Sequence,
            cancellationToken);

        if (sequenceExists)
        {
            throw new ConflictException(
                $"Process step sequence '{command.Sequence}' already exists in revision '{revision.RevisionCode}'.");
        }

        var processStep = ProcessStep.Create(
            revision.Id,
            command.Sequence,
            command.Name,
            command.Function,
            command.Requirement);

        _processStepRepository.Add(processStep);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProcessStepResult(
            processStep.Id,
            processStep.FmeaRevisionId,
            processStep.Sequence,
            processStep.Name,
            processStep.Function,
            processStep.Requirement);
    }
}
