using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.FailureAnalysis;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.CreateFailureCause;
using FmeaManager.Application.Fmeas.CreateFailureEffect;
using FmeaManager.Application.Fmeas.GetFailureCausesByFailureMode;
using FmeaManager.Application.Fmeas.GetFailureEffectsByFailureMode;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/failure-modes")]
public sealed class FailureModesController : ControllerBase
{
    private readonly GetFailureEffectsByFailureModeHandler _getEffectsHandler;
    private readonly CreateFailureEffectHandler _createEffectHandler;
    private readonly GetFailureCausesByFailureModeHandler _getCausesHandler;
    private readonly CreateFailureCauseHandler _createCauseHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public FailureModesController(
        GetFailureEffectsByFailureModeHandler getEffectsHandler,
        CreateFailureEffectHandler createEffectHandler,
        GetFailureCausesByFailureModeHandler getCausesHandler,
        CreateFailureCauseHandler createCauseHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _getEffectsHandler = getEffectsHandler;
        _createEffectHandler = createEffectHandler;
        _getCausesHandler = getCausesHandler;
        _createCauseHandler = createCauseHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet("{failureModeId:guid}/effects")]
    public async Task<ActionResult<IReadOnlyList<FailureEffectSummary>>> GetEffects(
        Guid failureModeId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFailureModeAccessAsync(
            _currentUser.UserKey,
            failureModeId,
            AccessPermission.Read,
            cancellationToken);

        return Ok(await _getEffectsHandler.HandleAsync(
            failureModeId,
            cancellationToken));
    }

    [HttpPost("{failureModeId:guid}/effects")]
    public async Task<ActionResult<CreateFailureEffectResult>> CreateEffect(
        Guid failureModeId,
        CreateFailureEffectRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFailureModeAccessAsync(
            _currentUser.UserKey,
            failureModeId,
            AccessPermission.Edit,
            cancellationToken);

        var effect = await _createEffectHandler.HandleAsync(
            new CreateFailureEffectCommand(
                failureModeId,
                request.Description),
            cancellationToken);

        await _auditService.RecordForFailureModeAsync(
            failureModeId,
            "Failure effect added",
            ProjectAuditAction.Edited,
            "FailureEffect",
            effect.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/failure-modes/{failureModeId}/effects",
            effect);
    }

    [HttpGet("{failureModeId:guid}/causes")]
    public async Task<ActionResult<IReadOnlyList<FailureCauseSummary>>> GetCauses(
        Guid failureModeId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFailureModeAccessAsync(
            _currentUser.UserKey,
            failureModeId,
            AccessPermission.Read,
            cancellationToken);

        return Ok(await _getCausesHandler.HandleAsync(
            failureModeId,
            cancellationToken));
    }

    [HttpPost("{failureModeId:guid}/causes")]
    public async Task<ActionResult<CreateFailureCauseResult>> CreateCause(
        Guid failureModeId,
        CreateFailureCauseRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFailureModeAccessAsync(
            _currentUser.UserKey,
            failureModeId,
            AccessPermission.Edit,
            cancellationToken);

        var cause = await _createCauseHandler.HandleAsync(
            new CreateFailureCauseCommand(
                failureModeId,
                request.Description),
            cancellationToken);

        await _auditService.RecordForFailureModeAsync(
            failureModeId,
            "Failure cause added",
            ProjectAuditAction.Edited,
            "FailureCause",
            cause.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/failure-modes/{failureModeId}/causes",
            cause);
    }
}
