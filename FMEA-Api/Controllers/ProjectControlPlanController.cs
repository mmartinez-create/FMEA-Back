using FMEA_Api.Contracts.ControlPlans;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Auditing;
using FmeaManager.Application.ControlPlans;
using FmeaManager.Domain.AccessControl;
using FmeaManager.Domain.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/control-plan")]
public sealed class ProjectControlPlanController : ControllerBase
{
    private readonly ControlPlanService _controlPlanService;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProjectControlPlanController(
        ControlPlanService controlPlanService,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _controlPlanService = controlPlanService;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ControlPlanWorkspaceResult?>> Get(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Read,
            cancellationToken);

        var result = await _controlPlanService.GetByProjectAsync(
            projectId,
            cancellationToken);

        return result is null ? NoContent() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ControlPlanWorkspaceResult>> Create(
        Guid projectId,
        CreateControlPlanRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _controlPlanService.CreateAsync(
            new CreateControlPlanCommand(
                projectId,
                request.Number,
                request.Name,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan created",
            ProjectAuditAction.Created,
            "ControlPlan",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/projects/{projectId}/control-plan",
            result);
    }

    [HttpPost("items")]
    public async Task<ActionResult<ControlPlanItemResult>> CreateItem(
        Guid projectId,
        SaveControlPlanItemRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _controlPlanService.CreateItemAsync(
            ToCommand(projectId, request, _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan characteristic added",
            ProjectAuditAction.Edited,
            "ControlPlanItem",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/projects/{projectId}/control-plan/items/{result.Id}",
            result);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<ControlPlanItemResult>> UpdateItem(
        Guid projectId,
        Guid itemId,
        SaveControlPlanItemRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _controlPlanService.UpdateItemAsync(
            itemId,
            ToCommand(projectId, request, _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan characteristic edited",
            ProjectAuditAction.Edited,
            "ControlPlanItem",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<ControlPlanWorkflowResult>> Submit(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _controlPlanService.SubmitAsync(
            projectId,
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan submitted for review",
            ProjectAuditAction.Submitted,
            "ControlPlan",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("approve")]
    public async Task<ActionResult<ControlPlanWorkflowResult>> Approve(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Approve,
            cancellationToken);

        var result = await _controlPlanService.ApproveAsync(
            projectId,
            _currentUser.UserKey,
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan approved",
            ProjectAuditAction.Approved,
            "ControlPlan",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("reject")]
    public async Task<ActionResult<ControlPlanWorkflowResult>> Reject(
        Guid projectId,
        RejectControlPlanRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Approve,
            cancellationToken);

        var result = await _controlPlanService.RejectAsync(
            new RejectControlPlanCommand(
                projectId,
                request.Reason,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "Control Plan rejected",
            ProjectAuditAction.Rejected,
            "ControlPlan",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }

    private static SaveControlPlanItemCommand ToCommand(
        Guid projectId,
        SaveControlPlanItemRequest request,
        string userKey) =>
        new(
            projectId,
            request.ProductProcessStepId,
            request.CharacteristicNumber,
            request.CharacteristicType,
            request.CharacteristicName,
            request.MachineTooling,
            request.SpecialCharacteristic,
            request.SpecificationTolerance,
            request.EvaluationMeasurementTechnique,
            request.SampleSize,
            request.SampleFrequency,
            request.ControlMethod,
            request.ReactionPlan,
            userKey);
}
