using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Auditing;
using FmeaManager.Application.DocumentTraceability;
using FmeaManager.Domain.AccessControl;
using FmeaManager.Domain.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/document-traceability")]
public sealed class ProjectDocumentTraceabilityController : ControllerBase
{
    private readonly ProjectDocumentTraceabilityService _traceabilityService;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public ProjectDocumentTraceabilityController(
        ProjectDocumentTraceabilityService traceabilityService,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _traceabilityService = traceabilityService;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ProjectDocumentTraceabilityResult>> Get(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Read,
            cancellationToken);

        return Ok(
            await _traceabilityService.GetAsync(
                projectId,
                cancellationToken));
    }

    [HttpPost("synchronize")]
    public async Task<ActionResult<SynchronizeProjectDocumentsResult>> Synchronize(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var result =
            await _traceabilityService.SynchronizeAsync(
                projectId,
                cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "ASMF, PFMEA and Control Plan synchronized",
            ProjectAuditAction.Edited,
            "ProjectDocumentChain",
            projectId,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Ok(result);
    }
}
