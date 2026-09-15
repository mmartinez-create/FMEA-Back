using FMEA_Api.Security;
using FmeaManager.Application.ProductStructure.GetProductStructure;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/product-structure")]
public sealed class ProductStructureController : ControllerBase
{
    private readonly GetProductStructureHandler _handler;
    private readonly CurrentUserAccessor _currentUser;

    public ProductStructureController(
        GetProductStructureHandler handler,
        CurrentUserAccessor currentUser)
    {
        _handler = handler;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(ProductStructureTree),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductStructureTree>> Get(
        CancellationToken cancellationToken)
    {
        var tree = await _handler.HandleAsync(
            _currentUser.UserKey,
            cancellationToken);

        return Ok(tree);
    }
}
