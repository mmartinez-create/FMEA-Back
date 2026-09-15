using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.FailureAnalysis;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.CreateDetectionControl;
using FmeaManager.Application.Fmeas.CreatePreventionControl;
using FmeaManager.Application.Fmeas.GetDetectionControlsByFailureCause;
using FmeaManager.Application.Fmeas.GetPreventionControlsByFailureCause;
using FmeaManager.Application.Fmeas.GetRiskAssessment;
using FmeaManager.Application.Fmeas.UpsertRiskAssessment;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/failure-causes")]
public sealed class FailureCausesController : ControllerBase
{
    private readonly GetPreventionControlsByFailureCauseHandler _getPreventionControlsHandler;
    private readonly CreatePreventionControlHandler _createPreventionControlHandler;
    private readonly GetDetectionControlsByFailureCauseHandler _getDetectionControlsHandler;
    private readonly CreateDetectionControlHandler _createDetectionControlHandler;
    private readonly GetRiskAssessmentHandler _getRiskAssessmentHandler;
    private readonly UpsertRiskAssessmentHandler _upsertRiskAssessmentHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public FailureCausesController(
        GetPreventionControlsByFailureCauseHandler getPreventionControlsHandler,
        CreatePreventionControlHandler createPreventionControlHandler,
        GetDetectionControlsByFailureCauseHandler getDetectionControlsHandler,
        CreateDetectionControlHandler createDetectionControlHandler,
        GetRiskAssessmentHandler getRiskAssessmentHandler,
        UpsertRiskAssessmentHandler upsertRiskAssessmentHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _getPreventionControlsHandler = getPreventionControlsHandler;
        _createPreventionControlHandler = createPreventionControlHandler;
        _getDetectionControlsHandler = getDetectionControlsHandler;
        _createDetectionControlHandler = createDetectionControlHandler;
        _getRiskAssessmentHandler = getRiskAssessmentHandler;
        _upsertRiskAssessmentHandler = upsertRiskAssessmentHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet("{failureCauseId:guid}/prevention-controls")]
    public async Task<ActionResult<IReadOnlyList<PreventionControlSummary>>> GetPreventionControls(
        Guid failureCauseId,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Read, cancellationToken);

        return Ok(await _getPreventionControlsHandler.HandleAsync(
            failureCauseId,
            cancellationToken));
    }

    [HttpPost("{failureCauseId:guid}/prevention-controls")]
    public async Task<ActionResult<CreatePreventionControlResult>> CreatePreventionControl(
        Guid failureCauseId,
        CreatePreventionControlRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Edit, cancellationToken);

        var control = await _createPreventionControlHandler.HandleAsync(
            new CreatePreventionControlCommand(
                failureCauseId,
                request.Description),
            cancellationToken);

        await _auditService.RecordForFailureCauseAsync(
            failureCauseId,
            "Prevention control added",
            ProjectAuditAction.Edited,
            "PreventionControl",
            control.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/failure-causes/{failureCauseId}/prevention-controls",
            control);
    }

    [HttpGet("{failureCauseId:guid}/detection-controls")]
    public async Task<ActionResult<IReadOnlyList<DetectionControlSummary>>> GetDetectionControls(
        Guid failureCauseId,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Read, cancellationToken);

        return Ok(await _getDetectionControlsHandler.HandleAsync(
            failureCauseId,
            cancellationToken));
    }

    [HttpPost("{failureCauseId:guid}/detection-controls")]
    public async Task<ActionResult<CreateDetectionControlResult>> CreateDetectionControl(
        Guid failureCauseId,
        CreateDetectionControlRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Edit, cancellationToken);

        var control = await _createDetectionControlHandler.HandleAsync(
            new CreateDetectionControlCommand(
                failureCauseId,
                request.Description),
            cancellationToken);

        await _auditService.RecordForFailureCauseAsync(
            failureCauseId,
            "Detection control added",
            ProjectAuditAction.Edited,
            "DetectionControl",
            control.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/failure-causes/{failureCauseId}/detection-controls",
            control);
    }

    [HttpGet("{failureCauseId:guid}/risk-assessment")]
    public async Task<ActionResult<RiskAssessmentResult?>> GetRiskAssessment(
        Guid failureCauseId,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Read, cancellationToken);

        var assessment = await _getRiskAssessmentHandler.HandleAsync(
            failureCauseId,
            cancellationToken);

        return assessment is null
            ? NoContent()
            : Ok(assessment);
    }

    [HttpPut("{failureCauseId:guid}/risk-assessment")]
    public async Task<ActionResult<RiskAssessmentResult>> UpsertRiskAssessment(
        Guid failureCauseId,
        UpsertRiskAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureAsync(failureCauseId, AccessPermission.Edit, cancellationToken);

        var assessment = await _upsertRiskAssessmentHandler.HandleAsync(
            new UpsertRiskAssessmentCommand(
                failureCauseId,
                request.Severity,
                request.Occurrence,
                request.Detection,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForFailureCauseAsync(
            failureCauseId,
            "Risk assessment updated",
            ProjectAuditAction.Edited,
            "RiskAssessment",
            assessment.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(assessment);
    }

    private Task EnsureAsync(
        Guid failureCauseId,
        AccessPermission permission,
        CancellationToken cancellationToken)
    {
        return _accessControl.EnsureFailureCauseAccessAsync(
            _currentUser.UserKey,
            failureCauseId,
            permission,
            cancellationToken);
    }
}
