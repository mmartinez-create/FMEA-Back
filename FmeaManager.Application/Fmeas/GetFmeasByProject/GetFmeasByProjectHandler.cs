using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetFmeasByProject;

public sealed class GetFmeasByProjectHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFmeaRepository _fmeaRepository;

    public GetFmeasByProjectHandler(
        IProjectRepository projectRepository,
        IFmeaRepository fmeaRepository)
    {
        _projectRepository = projectRepository;
        _fmeaRepository = fmeaRepository;
    }

    public async Task<IReadOnlyList<FmeaSummary>> HandleAsync(
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

        var fmeas = await _fmeaRepository.ListByProjectIdAsync(
            projectId,
            cancellationToken);

        return fmeas
            .Select(fmea => new FmeaSummary(
                fmea.Id,
                fmea.ProjectId,
                fmea.Number,
                fmea.Name,
                fmea.Type,
                fmea.Owner,
                fmea.IsActive,
                fmea.CreatedAt,
                fmea.UpdatedAt))
            .ToList();
    }
}
