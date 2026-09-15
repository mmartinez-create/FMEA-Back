using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.FailureAnalysis;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.CreateFailureMode;
using FmeaManager.Application.Fmeas.GetFailureModesByProcessStep;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/process-steps")]
public sealed class ProcessStepsController : ControllerBase
{
    private readonly GetFailureModesByProcessStepHandler _getFailureModesHandler;
    private readonly CreateFailureModeHandler _createFailureModeHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProcessStepsController(
        GetFailureModesByProcessStepHandler getFailureModesHandler,
        CreateFailureModeHandler createFailureModeHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _getFailureModesHandler = getFailureModesHandler;
        _createFailureModeHandler = createFailureModeHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet("{processStepId:guid}/failure-modes")]
    public async Task<ActionResult<IReadOnlyList<FailureModeSummary>>> GetFailureModes(
        Guid processStepId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProcessStepAccessAsync(
            _currentUser.UserKey,
            processStepId,
            AccessPermission.Read,
            cancellationToken);

        return Ok(await _getFailureModesHandler.HandleAsync(
            processStepId,
            cancellationToken));
    }

    [HttpPost("{processStepId:guid}/failure-modes")]
    public async Task<ActionResult<CreateFailureModeResult>> CreateFailureMode(
        Guid processStepId,
        CreateFailureModeRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProcessStepAccessAsync(
            _currentUser.UserKey,
            processStepId,
            AccessPermission.Edit,
            cancellationToken);

        var command = new CreateFailureModeCommand(
            processStepId,
            request.Description);

        var failureMode = await _createFailureModeHandler.HandleAsync(
            command,
            cancellationToken);

        await _auditService.RecordForProcessStepAsync(
            processStepId,
            "Failure mode added",
            ProjectAuditAction.Edited,
            "FailureMode",
            failureMode.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/process-steps/{processStepId}/failure-modes",
            failureMode);
    }
}
