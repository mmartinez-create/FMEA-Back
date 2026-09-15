using FmeaManager.Application.CustomerProfiles.GetCustomerProfiles;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/customer-profiles")]
public sealed class CustomerProfilesController : ControllerBase
{
    private readonly GetCustomerProfilesHandler _handler;

    public CustomerProfilesController(GetCustomerProfilesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerProfileSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerProfileSummary>>> GetAll(
        CancellationToken cancellationToken)
    {
        var profiles = await _handler.HandleAsync(cancellationToken);
        return Ok(profiles);
    }
}
