using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.ProductStructure;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.ProductStructure.CreateProjectForProduct;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly CreateProjectForProductHandler _createProjectHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProductsController(
        CreateProjectForProductHandler createProjectHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _createProjectHandler = createProjectHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpPost("{productId:guid}/projects")]
    [ProducesResponseType(typeof(CreateProjectForProductResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateProjectForProductResult>> CreateProject(
        Guid productId,
        CreateProjectForProductRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProductAccessAsync(
            _currentUser.UserKey,
            productId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createProjectHandler.HandleAsync(
            new CreateProjectForProductCommand(
                productId,
                request.Code,
                request.Name,
                request.CustomerProfileId,
                request.Owner,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            result.Id,
            "Project created",
            ProjectAuditAction.Created,
            "Project",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return CreatedAtAction(
            nameof(ProjectsController.GetById),
            "Projects",
            new { id = result.Id },
            result);
    }
}
