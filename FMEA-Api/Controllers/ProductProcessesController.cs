using FMEA_Api.Contracts.ProductProcesses;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Auditing;
using FmeaManager.Application.ProductProcesses.Common;
using FmeaManager.Application.ProductProcesses.CreateStep;
using FmeaManager.Domain.AccessControl;
using FmeaManager.Domain.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/product-processes")]
public sealed class ProductProcessesController : ControllerBase
{
    private readonly CreateProductProcessStepHandler _createStepHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProductProcessesController(
        CreateProductProcessStepHandler createStepHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _createStepHandler = createStepHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpPost("{productProcessId:guid}/steps")]
    [ProducesResponseType(
        typeof(ProductProcessStepResult),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductProcessStepResult>> CreateStep(
        Guid productProcessId,
        SaveProductProcessStepRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProductProcessAccessAsync(
            _currentUser.UserKey,
            productProcessId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createStepHandler.HandleAsync(
            new CreateProductProcessStepCommand(
                productProcessId,
                request.Sequence,
                request.Name,
                request.Function,
                request.Requirement,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            result.ProjectId,
            "ASMF process step added",
            ProjectAuditAction.Edited,
            "ProductProcessStep",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/product-process-steps/{result.Id}",
            result);
    }
}
