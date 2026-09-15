using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;

namespace FmeaManager.Application.Fmeas.GetFmeaById;

public sealed class GetFmeaByIdHandler
{
    private readonly IFmeaRepository _fmeaRepository;

    public GetFmeaByIdHandler(IFmeaRepository fmeaRepository)
    {
        _fmeaRepository = fmeaRepository;
    }

    public async Task<GetFmeaByIdResult> HandleAsync(
        Guid fmeaId,
        CancellationToken cancellationToken = default)
    {
        var fmea = await _fmeaRepository.GetByIdAsync(
            fmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{fmeaId}' was not found.");
        }

        return new GetFmeaByIdResult(
            fmea.Id,
            fmea.ProjectId,
            fmea.Number,
            fmea.Name,
            fmea.Type,
            fmea.Owner,
            fmea.IsActive,
            fmea.CreatedAt,
            fmea.CreatedBy,
            fmea.UpdatedAt);
    }
}
