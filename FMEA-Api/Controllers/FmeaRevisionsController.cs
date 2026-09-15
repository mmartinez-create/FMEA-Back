using FmeaManager.Application.Fmeas.SyncProductProcess;
using FmeaManager.Application.Fmeas.GetSyncedProcessSteps;
using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.Fmeas;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.ChangeRevisionStatus;
using FmeaManager.Application.Fmeas.CreateProcessStep;
using FmeaManager.Application.Fmeas.GetProcessStepsByRevision;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/fmea-revisions")]
public sealed class FmeaRevisionsController : ControllerBase
{
    private readonly GetProcessStepsHandler _getProcessStepsHandler;
    private readonly GetSyncedProcessStepsHandler _getSyncedProcessStepsHandler;
    private readonly SyncProductProcessHandler _syncProductProcessHandler;
    private readonly CreateProcessStepHandler _createProcessStepHandler;
    private readonly SubmitRevisionForReviewHandler _submitHandler;
    private readonly ApproveRevisionHandler _approveHandler;
    private readonly RejectRevisionHandler _rejectHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public FmeaRevisionsController(
        GetProcessStepsHandler getProcessStepsHandler,
        GetSyncedProcessStepsHandler getSyncedProcessStepsHandler,
        SyncProductProcessHandler syncProductProcessHandler,
        CreateProcessStepHandler createProcessStepHandler,
        SubmitRevisionForReviewHandler submitHandler,
        ApproveRevisionHandler approveHandler,
        RejectRevisionHandler rejectHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _getProcessStepsHandler = getProcessStepsHandler;
        _getSyncedProcessStepsHandler = getSyncedProcessStepsHandler;
        _syncProductProcessHandler = syncProductProcessHandler;
        _createProcessStepHandler = createProcessStepHandler;
        _submitHandler = submitHandler;
        _approveHandler = approveHandler;
        _rejectHandler = rejectHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet("{revisionId:guid}/process-steps")]
    public async Task<ActionResult<IReadOnlyList<ProcessStepSummary>>> GetProcessSteps(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Read,
            cancellationToken);

        var processSteps = await _getProcessStepsHandler.HandleAsync(
            revisionId,
            cancellationToken);

        return Ok(processSteps);
    }


    [HttpGet("{revisionId:guid}/synced-process-steps")]
    public async Task<ActionResult<IReadOnlyList<ProcessStepSyncSummary>>> GetSyncedProcessSteps(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Read,
            cancellationToken);

        return Ok(
            await _getSyncedProcessStepsHandler.HandleAsync(
                revisionId,
                cancellationToken));
    }

    [HttpPost("{revisionId:guid}/sync-product-process")]
    public async Task<ActionResult<SyncProductProcessResult>> SyncProductProcess(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Edit,
            cancellationToken);

        var result =
            await _syncProductProcessHandler.HandleAsync(
                revisionId,
                cancellationToken);

        if (
            result.CreatedCount > 0 ||
            result.LinkedExistingCount > 0 ||
            result.RefreshedCount > 0)
        {
            await _auditService.RecordForRevisionAsync(
                revisionId,
                "PFMEA process structure synchronized with ASMF",
                ProjectAuditAction.Edited,
                "FmeaRevision",
                revisionId,
                _currentUser.UserKey,
                _currentUser.DisplayName,
                cancellationToken);
        }

        return Ok(result);
    }

    [HttpPost("{revisionId:guid}/process-steps")]
    public async Task<ActionResult<CreateProcessStepResult>> CreateProcessStep(
        Guid revisionId,
        CreateProcessStepRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Edit,
            cancellationToken);

        var command = new CreateProcessStepCommand(
            revisionId,
            request.Sequence,
            request.Name,
            request.Function,
            request.Requirement);

        var processStep = await _createProcessStepHandler.HandleAsync(
            command,
            cancellationToken);

        await _auditService.RecordForRevisionAsync(
            revisionId,
            "Process step added",
            ProjectAuditAction.Edited,
            "ProcessStep",
            processStep.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/fmea-revisions/{revisionId}/process-steps",
            processStep);
    }

    [HttpPost("{revisionId:guid}/submit")]
    public async Task<ActionResult<RevisionStatusResult>> SubmitForReview(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _submitHandler.HandleAsync(
            revisionId,
            cancellationToken);

        await _auditService.RecordForRevisionAsync(
            revisionId,
            "PFMEA revision submitted for review",
            ProjectAuditAction.Submitted,
            "FmeaRevision",
            revisionId,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{revisionId:guid}/approve")]
    public async Task<ActionResult<RevisionStatusResult>> Approve(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Approve,
            cancellationToken);

        var result = await _approveHandler.HandleAsync(
            revisionId,
            _currentUser.UserKey,
            cancellationToken);

        await _auditService.RecordForRevisionAsync(
            revisionId,
            "PFMEA revision approved",
            ProjectAuditAction.Approved,
            "FmeaRevision",
            revisionId,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{revisionId:guid}/reject")]
    public async Task<ActionResult<RevisionStatusResult>> Reject(
        Guid revisionId,
        RejectRevisionRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureRevisionAccessAsync(
            _currentUser.UserKey,
            revisionId,
            AccessPermission.Approve,
            cancellationToken);

        var result = await _rejectHandler.HandleAsync(
            revisionId,
            request.Reason,
            cancellationToken);

        await _auditService.RecordForRevisionAsync(
            revisionId,
            "PFMEA revision rejected",
            ProjectAuditAction.Rejected,
            "FmeaRevision",
            revisionId,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }
}
