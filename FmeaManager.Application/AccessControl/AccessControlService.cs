using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.AccessControl;

namespace FmeaManager.Application.AccessControl;

public sealed class AccessControlService
{
    private readonly IPermissionAssignmentRepository _permissionRepository;
    private readonly IPlantRepository _plantRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IFmeaRepository _fmeaRepository;
    private readonly IFmeaRevisionRepository _revisionRepository;
    private readonly IProcessStepRepository _processStepRepository;
    private readonly IFailureModeRepository _failureModeRepository;
    private readonly IFailureCauseRepository _failureCauseRepository;
    private readonly IProductProcessRepository _productProcessRepository;
    private readonly IProductProcessStepRepository _productProcessStepRepository;

    public AccessControlService(
        IPermissionAssignmentRepository permissionRepository,
        IPlantRepository plantRepository,
        IProductionLineRepository productionLineRepository,
        IProductRepository productRepository,
        IProjectRepository projectRepository,
        IFmeaRepository fmeaRepository,
        IFmeaRevisionRepository revisionRepository,
        IProcessStepRepository processStepRepository,
        IFailureModeRepository failureModeRepository,
        IFailureCauseRepository failureCauseRepository,
        IProductProcessRepository productProcessRepository,
        IProductProcessStepRepository productProcessStepRepository)
    {
        _permissionRepository = permissionRepository;
        _plantRepository = plantRepository;
        _productionLineRepository = productionLineRepository;
        _productRepository = productRepository;
        _projectRepository = projectRepository;
        _fmeaRepository = fmeaRepository;
        _revisionRepository = revisionRepository;
        _processStepRepository = processStepRepository;
        _failureModeRepository = failureModeRepository;
        _failureCauseRepository = failureCauseRepository;
        _productProcessRepository = productProcessRepository;
        _productProcessStepRepository = productProcessStepRepository;
    }

    public Task<EffectiveAccess> GetSystemAccessAsync(
        string userKey,
        CancellationToken cancellationToken = default)
    {
        return EvaluateAsync(
            userKey,
            new[] { Scope(PermissionScopeType.System, Guid.Empty) },
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetPlantAccessAsync(
        string userKey,
        Guid plantId,
        CancellationToken cancellationToken = default)
    {
        var plant = await _plantRepository.GetByIdAsync(
            plantId,
            cancellationToken);

        if (plant is null)
        {
            throw new NotFoundException(
                $"Plant '{plantId}' was not found.");
        }

        return await EvaluateAsync(
            userKey,
            new[]
            {
                Scope(PermissionScopeType.System, Guid.Empty),
                Scope(PermissionScopeType.Plant, plant.Id)
            },
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetProductionLineAccessAsync(
        string userKey,
        Guid productionLineId,
        CancellationToken cancellationToken = default)
    {
        var line = await _productionLineRepository.GetByIdAsync(
            productionLineId,
            cancellationToken);

        if (line is null)
        {
            throw new NotFoundException(
                $"Production line '{productionLineId}' was not found.");
        }

        return await EvaluateAsync(
            userKey,
            new[]
            {
                Scope(PermissionScopeType.System, Guid.Empty),
                Scope(PermissionScopeType.Plant, line.PlantId),
                Scope(PermissionScopeType.ProductionLine, line.Id)
            },
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetProductAccessAsync(
        string userKey,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            productId,
            cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product '{productId}' was not found.");
        }

        var line = await _productionLineRepository.GetByIdAsync(
            product.ProductionLineId,
            cancellationToken);

        if (line is null)
        {
            throw new NotFoundException(
                $"Production line '{product.ProductionLineId}' was not found.");
        }

        return await EvaluateAsync(
            userKey,
            new[]
            {
                Scope(PermissionScopeType.System, Guid.Empty),
                Scope(PermissionScopeType.Plant, line.PlantId),
                Scope(PermissionScopeType.ProductionLine, line.Id),
                Scope(PermissionScopeType.Product, product.Id)
            },
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetProjectAccessAsync(
        string userKey,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(
            projectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                $"Project '{projectId}' was not found.");
        }

        var scopes = new List<(PermissionScopeType Type, Guid Id)>
        {
            Scope(PermissionScopeType.System, Guid.Empty)
        };

        if (project.ProductId.HasValue)
        {
            var product = await _productRepository.GetByIdAsync(
                project.ProductId.Value,
                cancellationToken);

            if (product is not null)
            {
                var line = await _productionLineRepository.GetByIdAsync(
                    product.ProductionLineId,
                    cancellationToken);

                if (line is not null)
                {
                    scopes.Add(
                        Scope(PermissionScopeType.Plant, line.PlantId));
                    scopes.Add(
                        Scope(PermissionScopeType.ProductionLine, line.Id));
                }

                scopes.Add(
                    Scope(PermissionScopeType.Product, product.Id));
            }
        }

        scopes.Add(Scope(PermissionScopeType.Project, project.Id));

        return await EvaluateAsync(
            userKey,
            scopes,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetFmeaAccessAsync(
        string userKey,
        Guid fmeaId,
        CancellationToken cancellationToken = default)
    {
        var fmea = await _fmeaRepository.GetByIdAsync(
            fmeaId,
            cancellationToken);

        if (fmea is null)
        {
            throw new NotFoundException(
                $"FMEA '{fmeaId}' was not found.");
        }

        return await GetProjectAccessAsync(
            userKey,
            fmea.ProjectId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetRevisionAccessAsync(
        string userKey,
        Guid revisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _revisionRepository.GetByIdAsync(
            revisionId,
            cancellationToken);

        if (revision is null)
        {
            throw new NotFoundException(
                $"FMEA revision '{revisionId}' was not found.");
        }

        return await GetFmeaAccessAsync(
            userKey,
            revision.FmeaId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetProcessStepAccessAsync(
        string userKey,
        Guid processStepId,
        CancellationToken cancellationToken = default)
    {
        var step = await _processStepRepository.GetByIdAsync(
            processStepId,
            cancellationToken);

        if (step is null)
        {
            throw new NotFoundException(
                $"Process step '{processStepId}' was not found.");
        }

        return await GetRevisionAccessAsync(
            userKey,
            step.FmeaRevisionId,
            cancellationToken);
    }


    public async Task<EffectiveAccess> GetProductProcessAccessAsync(
        string userKey,
        Guid productProcessId,
        CancellationToken cancellationToken = default)
    {
        var process = await _productProcessRepository.GetByIdAsync(
            productProcessId,
            cancellationToken);

        if (process is null)
        {
            throw new NotFoundException(
                $"Product process '{productProcessId}' was not found.");
        }

        return await GetProjectAccessAsync(
            userKey,
            process.ProjectId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetProductProcessStepAccessAsync(
        string userKey,
        Guid productProcessStepId,
        CancellationToken cancellationToken = default)
    {
        var step = await _productProcessStepRepository.GetByIdAsync(
            productProcessStepId,
            cancellationToken);

        if (step is null)
        {
            throw new NotFoundException(
                $"Product process step '{productProcessStepId}' was not found.");
        }

        return await GetProductProcessAccessAsync(
            userKey,
            step.ProductProcessId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetFailureModeAccessAsync(
        string userKey,
        Guid failureModeId,
        CancellationToken cancellationToken = default)
    {
        var mode = await _failureModeRepository.GetByIdAsync(
            failureModeId,
            cancellationToken);

        if (mode is null)
        {
            throw new NotFoundException(
                $"Failure mode '{failureModeId}' was not found.");
        }

        return await GetProcessStepAccessAsync(
            userKey,
            mode.ProcessStepId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetFailureCauseAccessAsync(
        string userKey,
        Guid failureCauseId,
        CancellationToken cancellationToken = default)
    {
        var cause = await _failureCauseRepository.GetByIdAsync(
            failureCauseId,
            cancellationToken);

        if (cause is null)
        {
            throw new NotFoundException(
                $"Failure cause '{failureCauseId}' was not found.");
        }

        return await GetFailureModeAccessAsync(
            userKey,
            cause.FailureModeId,
            cancellationToken);
    }

    public async Task<EffectiveAccess> GetScopeAccessAsync(
        string userKey,
        PermissionScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken = default)
    {
        return scopeType switch
        {
            PermissionScopeType.System =>
                await GetSystemAccessAsync(userKey, cancellationToken),

            PermissionScopeType.Plant =>
                await GetPlantAccessAsync(userKey, scopeId, cancellationToken),

            PermissionScopeType.ProductionLine =>
                await GetProductionLineAccessAsync(userKey, scopeId, cancellationToken),

            PermissionScopeType.Product =>
                await GetProductAccessAsync(userKey, scopeId, cancellationToken),

            PermissionScopeType.Project =>
                await GetProjectAccessAsync(userKey, scopeId, cancellationToken),

            _ => throw new ArgumentOutOfRangeException(
                nameof(scopeType),
                scopeType,
                "Permission scope type is invalid.")
        };
    }

    public async Task EnsureSystemAccessAsync(
        string userKey,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetSystemAccessAsync(
            userKey,
            cancellationToken);

        Ensure(access, required, "system");
    }

    public async Task EnsurePlantAccessAsync(
        string userKey,
        Guid plantId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetPlantAccessAsync(
            userKey,
            plantId,
            cancellationToken);

        Ensure(access, required, $"plant '{plantId}'");
    }

    public async Task EnsureProductionLineAccessAsync(
        string userKey,
        Guid productionLineId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProductionLineAccessAsync(
            userKey,
            productionLineId,
            cancellationToken);

        Ensure(access, required, $"production line '{productionLineId}'");
    }

    public async Task EnsureProductAccessAsync(
        string userKey,
        Guid productId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProductAccessAsync(
            userKey,
            productId,
            cancellationToken);

        Ensure(access, required, $"product '{productId}'");
    }

    public async Task EnsureProjectAccessAsync(
        string userKey,
        Guid projectId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProjectAccessAsync(
            userKey,
            projectId,
            cancellationToken);

        Ensure(access, required, $"project '{projectId}'");
    }

    public async Task EnsureFmeaAccessAsync(
        string userKey,
        Guid fmeaId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetFmeaAccessAsync(
            userKey,
            fmeaId,
            cancellationToken);

        Ensure(access, required, $"FMEA '{fmeaId}'");
    }

    public async Task EnsureRevisionAccessAsync(
        string userKey,
        Guid revisionId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetRevisionAccessAsync(
            userKey,
            revisionId,
            cancellationToken);

        Ensure(access, required, $"revision '{revisionId}'");
    }

    public async Task EnsureProcessStepAccessAsync(
        string userKey,
        Guid processStepId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProcessStepAccessAsync(
            userKey,
            processStepId,
            cancellationToken);

        Ensure(access, required, $"process step '{processStepId}'");
    }


    public async Task EnsureProductProcessAccessAsync(
        string userKey,
        Guid productProcessId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProductProcessAccessAsync(
            userKey,
            productProcessId,
            cancellationToken);

        Ensure(
            access,
            required,
            $"product process '{productProcessId}'");
    }

    public async Task EnsureProductProcessStepAccessAsync(
        string userKey,
        Guid productProcessStepId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetProductProcessStepAccessAsync(
            userKey,
            productProcessStepId,
            cancellationToken);

        Ensure(
            access,
            required,
            $"product process step '{productProcessStepId}'");
    }

    public async Task EnsureFailureModeAccessAsync(
        string userKey,
        Guid failureModeId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetFailureModeAccessAsync(
            userKey,
            failureModeId,
            cancellationToken);

        Ensure(access, required, $"failure mode '{failureModeId}'");
    }

    public async Task EnsureFailureCauseAccessAsync(
        string userKey,
        Guid failureCauseId,
        AccessPermission required,
        CancellationToken cancellationToken = default)
    {
        var access = await GetFailureCauseAccessAsync(
            userKey,
            failureCauseId,
            cancellationToken);

        Ensure(access, required, $"failure cause '{failureCauseId}'");
    }

    private async Task<EffectiveAccess> EvaluateAsync(
        string userKey,
        IEnumerable<(PermissionScopeType Type, Guid Id)> scopes,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeUserKey(userKey);

        var assignments =
            await _permissionRepository.ListActiveByUserAsync(
                normalized,
                cancellationToken);

        var scopeList = scopes.ToList();

        var applicable = assignments
            .Where(assignment =>
                scopeList.Any(scope =>
                    scope.Type == assignment.ScopeType &&
                    scope.Id == assignment.ScopeId))
            .ToList();

        if (applicable.Count == 0)
        {
            return EffectiveAccess.None;
        }

        var permissions = applicable.Aggregate(
            AccessPermission.None,
            (current, assignment) =>
                current | assignment.Permissions);

        var sources = applicable
            .Select(assignment => new PermissionSource(
                assignment.ScopeType,
                assignment.ScopeId,
                assignment.Permissions))
            .ToList();

        return new EffectiveAccess(
            permissions.HasFlag(AccessPermission.Read),
            permissions.HasFlag(AccessPermission.Edit),
            permissions.HasFlag(AccessPermission.Approve),
            permissions,
            sources);
    }

    private static void Ensure(
        EffectiveAccess access,
        AccessPermission required,
        string resource)
    {
        if (!access.Permissions.HasFlag(required))
        {
            throw new ForbiddenException(
                $"You do not have {required} permission for {resource}.");
        }
    }

    private static (PermissionScopeType Type, Guid Id) Scope(
        PermissionScopeType type,
        Guid id)
    {
        return (type, id);
    }

    private static string NormalizeUserKey(string userKey)
    {
        if (string.IsNullOrWhiteSpace(userKey))
        {
            throw new ArgumentException(
                "User key is required.",
                nameof(userKey));
        }

        return userKey.Trim().ToLowerInvariant();
    }
}
