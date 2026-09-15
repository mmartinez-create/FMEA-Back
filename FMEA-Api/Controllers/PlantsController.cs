using FMEA_Api.Contracts.ProductStructure;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.ProductStructure.CreatePlant;
using FmeaManager.Application.ProductStructure.CreateProductionLine;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/plants")]
public sealed class PlantsController : ControllerBase
{
    private readonly CreatePlantHandler _createPlantHandler;
    private readonly CreateProductionLineHandler _createProductionLineHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;

    public PlantsController(
        CreatePlantHandler createPlantHandler,
        CreateProductionLineHandler createProductionLineHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser)
    {
        _createPlantHandler = createPlantHandler;
        _createProductionLineHandler = createProductionLineHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatePlantResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreatePlantResult>> Create(
        CreatePlantRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureSystemAccessAsync(
            _currentUser.UserKey,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createPlantHandler.HandleAsync(
            new CreatePlantCommand(
                request.Code,
                request.Name),
            cancellationToken);

        return Created(
            $"/api/plants/{result.Id}",
            result);
    }

    [HttpPost("{plantId:guid}/production-lines")]
    [ProducesResponseType(typeof(CreateProductionLineResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateProductionLineResult>> CreateLine(
        Guid plantId,
        CreateProductionLineRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsurePlantAccessAsync(
            _currentUser.UserKey,
            plantId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createProductionLineHandler.HandleAsync(
            new CreateProductionLineCommand(
                plantId,
                request.Code,
                request.Name),
            cancellationToken);

        return Created(
            $"/api/plants/{plantId}/production-lines/{result.Id}",
            result);
    }
}
