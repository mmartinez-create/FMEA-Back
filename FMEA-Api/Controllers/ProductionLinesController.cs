using FMEA_Api.Contracts.ProductStructure;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.ProductStructure.CreateProduct;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/production-lines")]
public sealed class ProductionLinesController : ControllerBase
{
    private readonly CreateProductHandler _createProductHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;

    public ProductionLinesController(
        CreateProductHandler createProductHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser)
    {
        _createProductHandler = createProductHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
    }

    [HttpPost("{productionLineId:guid}/products")]
    [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateProductResult>> CreateProduct(
        Guid productionLineId,
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProductionLineAccessAsync(
            _currentUser.UserKey,
            productionLineId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createProductHandler.HandleAsync(
            new CreateProductCommand(
                productionLineId,
                request.Code,
                request.Name,
                request.PartNumber),
            cancellationToken);

        return Created(
            $"/api/production-lines/{productionLineId}/products/{result.Id}",
            result);
    }
}
