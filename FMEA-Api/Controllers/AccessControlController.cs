using FMEA_Api.Contracts.AccessControl;
using FMEA_Api.Security;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.AccessControl.GetUserAssignments;
using FmeaManager.Application.AccessControl.RevokePermissionAssignment;
using FmeaManager.Application.AccessControl.UpsertPermissionAssignment;
using FmeaManager.Domain.AccessControl;
using Microsoft.AspNetCore.Mvc;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/access-control")]
public sealed class AccessControlController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUser;
    private readonly AccessControlService _accessControl;
    private readonly UpsertPermissionAssignmentHandler _upsertHandler;
    private readonly GetUserAssignmentsHandler _getAssignmentsHandler;
    private readonly RevokePermissionAssignmentHandler _revokeHandler;

    public AccessControlController(
        CurrentUserAccessor currentUser,
        AccessControlService accessControl,
        UpsertPermissionAssignmentHandler upsertHandler,
        GetUserAssignmentsHandler getAssignmentsHandler,
        RevokePermissionAssignmentHandler revokeHandler)
    {
        _currentUser = currentUser;
        _accessControl = accessControl;
        _upsertHandler = upsertHandler;
        _getAssignmentsHandler = getAssignmentsHandler;
        _revokeHandler = revokeHandler;
    }

    [HttpGet("me")]
    public ActionResult<object> GetCurrentUser()
    {
        return Ok(new
        {
            userKey = _currentUser.UserKey
        });
    }

    [HttpGet("effective")]
    [ProducesResponseType(typeof(EffectiveAccess), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EffectiveAccess>> GetEffective(
        PermissionScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken)
    {
        var access = await _accessControl.GetScopeAccessAsync(
            _currentUser.UserKey,
            scopeType,
            scopeId,
            cancellationToken);

        return Ok(access);
    }

    [HttpGet("users/{userKey}/assignments")]
    [ProducesResponseType(
        typeof(IReadOnlyList<PermissionAssignmentResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionAssignmentResult>>>
        GetAssignments(
            string userKey,
            CancellationToken cancellationToken)
    {
        var assignments =
            await _getAssignmentsHandler.HandleAsync(
                userKey,
                _currentUser.UserKey,
                cancellationToken);

        return Ok(assignments);
    }

    [HttpPut("users/{userKey}/assignments")]
    [ProducesResponseType(
        typeof(PermissionAssignmentResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionAssignmentResult>>
        UpsertAssignment(
            string userKey,
            UpsertPermissionAssignmentRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _upsertHandler.HandleAsync(
            new UpsertPermissionAssignmentCommand(
                userKey,
                request.ScopeType,
                request.ScopeId,
                request.CanRead,
                request.CanEdit,
                request.CanApprove,
                _currentUser.UserKey),
            cancellationToken);

        return Ok(result);
    }


    [HttpDelete("users/{userKey}/assignments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAssignment(
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken)
    {
        await _revokeHandler.HandleAsync(
            new RevokePermissionAssignmentCommand(
                userKey,
                scopeType,
                scopeId,
                _currentUser.UserKey),
            cancellationToken);

        return NoContent();
    }

}
