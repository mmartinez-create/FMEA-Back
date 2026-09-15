using FMEA_Api.Contracts.ProductProcesses;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Auditing;
using FmeaManager.Application.ProductProcesses.Common;
using FmeaManager.Application.ProductProcesses.UpdateStep;
using FmeaManager.Domain.AccessControl;
using FmeaManager.Domain.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/product-process-steps")]
public sealed class ProductProcessStepsController : ControllerBase
{
    private readonly UpdateProductProcessStepHandler _updateStepHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProductProcessStepsController(
        UpdateProductProcessStepHandler updateStepHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _updateStepHandler = updateStepHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpPut("{stepId:guid}")]
    [ProducesResponseType(
        typeof(ProductProcessStepResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductProcessStepResult>> Update(
        Guid stepId,
        SaveProductProcessStepRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProductProcessStepAccessAsync(
            _currentUser.UserKey,
            stepId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _updateStepHandler.HandleAsync(
            new UpdateProductProcessStepCommand(
                stepId,
                request.Sequence,
                request.Name,
                request.Function,
                request.Requirement,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            result.ProjectId,
            "ASMF process step edited",
            ProjectAuditAction.Edited,
            "ProductProcessStep",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }
}
