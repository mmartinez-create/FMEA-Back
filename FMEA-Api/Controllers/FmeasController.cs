using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.Fmeas;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.CreateRevision;
using FmeaManager.Application.Fmeas.GetFmeaById;
using FmeaManager.Application.Fmeas.GetRevisionsByFmea;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/fmeas")]
public sealed class FmeasController : ControllerBase
{
    private readonly GetFmeaByIdHandler _getFmeaByIdHandler;
    private readonly GetFmeaRevisionsHandler _getFmeaRevisionsHandler;
    private readonly CreateFmeaRevisionHandler _createFmeaRevisionHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;

    public FmeasController(
        GetFmeaByIdHandler getFmeaByIdHandler,
        GetFmeaRevisionsHandler getFmeaRevisionsHandler,
        CreateFmeaRevisionHandler createFmeaRevisionHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService)
    {
        _getFmeaByIdHandler = getFmeaByIdHandler;
        _getFmeaRevisionsHandler = getFmeaRevisionsHandler;
        _createFmeaRevisionHandler = createFmeaRevisionHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet("{fmeaId:guid}")]
    public async Task<ActionResult<GetFmeaByIdResult>> GetById(
        Guid fmeaId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFmeaAccessAsync(
            _currentUser.UserKey,
            fmeaId,
            AccessPermission.Read,
            cancellationToken);

        var fmea = await _getFmeaByIdHandler.HandleAsync(
            fmeaId,
            cancellationToken);

        return Ok(fmea);
    }

    [HttpGet("{fmeaId:guid}/revisions")]
    public async Task<ActionResult<IReadOnlyList<FmeaRevisionSummary>>> GetRevisions(
        Guid fmeaId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFmeaAccessAsync(
            _currentUser.UserKey,
            fmeaId,
            AccessPermission.Read,
            cancellationToken);

        var revisions = await _getFmeaRevisionsHandler.HandleAsync(
            fmeaId,
            cancellationToken);

        return Ok(revisions);
    }

    [HttpPost("{fmeaId:guid}/revisions")]
    public async Task<ActionResult<CreateFmeaRevisionResult>> CreateRevision(
        Guid fmeaId,
        CreateFmeaRevisionRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureFmeaAccessAsync(
            _currentUser.UserKey,
            fmeaId,
            AccessPermission.Edit,
            cancellationToken);

        var command = new CreateFmeaRevisionCommand(
            fmeaId,
            request.BasedOnRevisionId,
            request.RevisionReason,
            _currentUser.UserKey);

        var revision = await _createFmeaRevisionHandler.HandleAsync(
            command,
            cancellationToken);

        await _auditService.RecordForFmeaAsync(
            fmeaId,
            "PFMEA revision created",
            ProjectAuditAction.Created,
            "FmeaRevision",
            revision.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/fmeas/{fmeaId}/revisions",
            revision);
    }
}
