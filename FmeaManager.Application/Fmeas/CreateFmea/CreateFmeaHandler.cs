using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.Fmeas.CreateFmea;

public sealed class CreateFmeaHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFmeaHandler(
        IProjectRepository projectRepository,
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFmeaResult> HandleAsync(
        CreateFmeaCommand command,
        CancellationToken cancellationToken = default)
    {
        var fmea = Fmea.Create(
            command.ProjectId,
            command.Number,
            command.Name,
            command.Type,
            command.Owner,
            command.CreatedBy);

        var project = await _projectRepository.GetByIdAsync(
            fmea.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{fmea.ProjectId}' was not found.");
        }

        if (project.Status == ProjectStatus.Archived)
        {
            throw new ConflictException(
                $"Project '{project.Code}' is archived and cannot receive new FMEAs.");
        }

        var numberExists = await _fmeaRepository.ExistsByNumberAsync(
            fmea.ProjectId,
            fmea.Number,
            cancellationToken);

        if (numberExists)
        {
            throw new ConflictException(
                $"FMEA number '{fmea.Number}' already exists in project '{project.Code}'.");
        }

        var initialRevision = FmeaRevision.CreateInitial(
            fmea.Id,
            command.CreatedBy);

        _fmeaRepository.Add(fmea);
        _revisionRepository.Add(initialRevision);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateFmeaResult(
            fmea.Id,
            fmea.ProjectId,
            fmea.Number,
            fmea.Name,
            fmea.Type,
            fmea.Owner,
            initialRevision.Id,
            initialRevision.RevisionCode,
            initialRevision.Status,
            fmea.CreatedAt,
            fmea.CreatedBy);
    }
}
