using FmeaManager.Application.ProductProcesses.GetByProject;
using FmeaManager.Application.ProductProcesses.Create;
using FmeaManager.Application.ProductProcesses.Common;
using FMEA_Api.Contracts.ProductProcesses;
using FmeaManager.Domain.Auditing;
using FmeaManager.Application.Auditing;
using FMEA_Api.Contracts.Fmeas;
using FMEA_Api.Contracts.Projects;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.Fmeas.CreateFmea;
using FmeaManager.Application.Fmeas.GetFmeasByProject;
using FmeaManager.Application.Projects.CreateProject;
using FmeaManager.Application.Projects.GetProjectById;
using FmeaManager.Application.Projects.GetProjects;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly CreateProjectHandler _createProjectHandler;
    private readonly GetProjectsHandler _getProjectsHandler;
    private readonly GetProjectByIdHandler _getProjectByIdHandler;
    private readonly CreateFmeaHandler _createFmeaHandler;
    private readonly GetFmeasByProjectHandler _getFmeasByProjectHandler;
    private readonly AccessControlService _accessControl;
    private readonly CurrentUserAccessor _currentUser;
    private readonly ProjectAuditService _auditService;
    private readonly GetLastProjectActivityHandler _lastActivityHandler;
    private readonly GetProductProcessByProjectHandler _getProductProcessHandler;
    private readonly CreateProductProcessHandler _createProductProcessHandler;

    public ProjectsController(
        CreateProjectHandler createProjectHandler,
        GetProjectsHandler getProjectsHandler,
        GetProjectByIdHandler getProjectByIdHandler,
        CreateFmeaHandler createFmeaHandler,
        GetFmeasByProjectHandler getFmeasByProjectHandler,
        AccessControlService accessControl,
        CurrentUserAccessor currentUser,
        ProjectAuditService auditService,
        GetLastProjectActivityHandler lastActivityHandler,
        GetProductProcessByProjectHandler getProductProcessHandler,
        CreateProductProcessHandler createProductProcessHandler)
    {
        _createProjectHandler = createProjectHandler;
        _getProjectsHandler = getProjectsHandler;
        _getProjectByIdHandler = getProjectByIdHandler;
        _createFmeaHandler = createFmeaHandler;
        _getFmeasByProjectHandler = getFmeasByProjectHandler;
        _accessControl = accessControl;
        _currentUser = currentUser;
        _auditService = auditService;
        _lastActivityHandler = lastActivityHandler;
        _getProductProcessHandler = getProductProcessHandler;
        _createProductProcessHandler = createProductProcessHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectSummary>>> GetAll(
        CancellationToken cancellationToken)
    {
        var projects = await _getProjectsHandler.HandleAsync(cancellationToken);
        var visible = new List<ProjectSummary>();

        foreach (var project in projects)
        {
            var access = await _accessControl.GetProjectAccessAsync(
                _currentUser.UserKey,
                project.Id,
                cancellationToken);

            if (access.CanRead)
            {
                visible.Add(project);
            }
        }

        return Ok(visible);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetProjectByIdResult>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            id,
            AccessPermission.Read,
            cancellationToken);

        var project = await _getProjectByIdHandler.HandleAsync(
            id,
            cancellationToken);

        return Ok(project);
    }


    [HttpGet("{id:guid}/last-activity")]
    [ProducesResponseType(typeof(LastProjectActivityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LastProjectActivityResult?>> GetLastActivity(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            id,
            AccessPermission.Read,
            cancellationToken);

        var activity = await _lastActivityHandler.HandleAsync(
            id,
            cancellationToken);

        return activity is null
            ? NoContent()
            : Ok(activity);
    }


    [HttpGet("{id:guid}/product-process")]
    [ProducesResponseType(
        typeof(ProductProcessWorkspaceResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductProcessWorkspaceResult?>> GetProductProcess(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            id,
            AccessPermission.Read,
            cancellationToken);

        var result = await _getProductProcessHandler.HandleAsync(
            id,
            cancellationToken);

        return result is null
            ? NoContent()
            : Ok(result);
    }

    [HttpPost("{id:guid}/product-process")]
    [ProducesResponseType(
        typeof(ProductProcessWorkspaceResult),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductProcessWorkspaceResult>> CreateProductProcess(
        Guid id,
        CreateProductProcessRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            id,
            AccessPermission.Edit,
            cancellationToken);

        var result = await _createProductProcessHandler.HandleAsync(
            new CreateProductProcessCommand(
                id,
                request.Name,
                request.Description,
                _currentUser.UserKey),
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            id,
            "ASMF product process created",
            ProjectAuditAction.Created,
            "ProductProcess",
            result.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return Created(
            $"/api/projects/{id}/product-process",
            result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateProjectResult>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        // Legacy free-text project creation is now system-level.
        await _accessControl.EnsureSystemAccessAsync(
            _currentUser.UserKey,
            AccessPermission.Edit,
            cancellationToken);

        var command = new CreateProjectCommand(
            request.Code,
            request.Name,
            request.Product,
            request.Plant,
            request.CustomerProfileId,
            request.Owner,
            _currentUser.UserKey);

        var project = await _createProjectHandler.HandleAsync(
            command,
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            project.Id,
            "Project created",
            ProjectAuditAction.Created,
            "Project",
            project.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            project);
    }

    [HttpGet("{projectId:guid}/fmeas")]
    public async Task<ActionResult<IReadOnlyList<FmeaSummary>>> GetFmeas(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Read,
            cancellationToken);

        var fmeas = await _getFmeasByProjectHandler.HandleAsync(
            projectId,
            cancellationToken);

        return Ok(fmeas);
    }

    [HttpPost("{projectId:guid}/fmeas")]
    public async Task<ActionResult<CreateFmeaResult>> CreateFmea(
        Guid projectId,
        CreateFmeaRequest request,
        CancellationToken cancellationToken)
    {
        await _accessControl.EnsureProjectAccessAsync(
            _currentUser.UserKey,
            projectId,
            AccessPermission.Edit,
            cancellationToken);

        var command = new CreateFmeaCommand(
            projectId,
            request.Number,
            request.Name,
            request.Type,
            request.Owner,
            _currentUser.UserKey);

        var fmea = await _createFmeaHandler.HandleAsync(
            command,
            cancellationToken);

        await _auditService.RecordForProjectAsync(
            projectId,
            "PFMEA created",
            ProjectAuditAction.Created,
            "Fmea",
            fmea.Id,
            _currentUser.UserKey,
            _currentUser.DisplayName,
            cancellationToken);

        return CreatedAtAction(
            nameof(FmeasController.GetById),
            "Fmeas",
            new { fmeaId = fmea.Id },
            fmea);
    }
}
