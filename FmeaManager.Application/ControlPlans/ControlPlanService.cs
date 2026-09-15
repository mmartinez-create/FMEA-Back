using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.ProductStructure;
using FmeaManager.Domain.Projects;

namespace FmeaManager.Application.ControlPlans;

public sealed class ControlPlanService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IControlPlanRepository _controlPlanRepository;
    private readonly IControlPlanItemRepository _itemRepository;
    private readonly IProductProcessRepository _productProcessRepository;
    private readonly IProductProcessStepRepository _sharedStepRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ControlPlanService(
        IProjectRepository projectRepository,
        IControlPlanRepository controlPlanRepository,
        IControlPlanItemRepository itemRepository,
        IProductProcessRepository productProcessRepository,
        IProductProcessStepRepository sharedStepRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _controlPlanRepository = controlPlanRepository;
        _itemRepository = itemRepository;
        _productProcessRepository = productProcessRepository;
        _sharedStepRepository = sharedStepRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ControlPlanWorkspaceResult?> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        await EnsureProjectExistsAsync(projectId, cancellationToken);

        var plan = await _controlPlanRepository.GetByProjectIdAsync(
            projectId, cancellationToken);

        return plan is null
            ? null
            : await MapWorkspaceAsync(plan, cancellationToken);
    }

    public async Task<ControlPlanWorkspaceResult> CreateAsync(
        CreateControlPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var project = await EnsureProjectExistsAsync(
            command.ProjectId, cancellationToken);

        if (!project.ProductId.HasValue)
            throw new ConflictException(
                "The project must be linked to a Product before creating a Control Plan.");

        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                command.ProjectId, cancellationToken);

        if (sharedProcess is null)
            throw new ConflictException(
                "Create the ASMF / Product Process before creating the Control Plan.");

        var existing =
            await _controlPlanRepository.GetByProjectIdAsync(
                command.ProjectId, cancellationToken);

        if (existing is not null)
            throw new ConflictException(
                "This project already has a Control Plan.");

        var plan = ControlPlan.Create(
            command.ProjectId,
            command.Number,
            command.Name,
            command.CreatedBy);

        _controlPlanRepository.Add(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapWorkspaceAsync(plan, cancellationToken);
    }

    public async Task<ControlPlanItemResult> CreateItemAsync(
        SaveControlPlanItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanForProjectAsync(
            command.ProjectId, cancellationToken);

        plan.EnsureEditable();

        var sharedStep = await GetSharedStepForProjectAsync(
            command.ProjectId,
            command.ProductProcessStepId,
            cancellationToken);

        var item = ControlPlanItem.Create(
            plan.Id,
            sharedStep,
            command.CharacteristicNumber,
            command.CharacteristicType,
            command.CharacteristicName,
            command.MachineTooling,
            command.SpecialCharacteristic,
            command.SpecificationTolerance,
            command.EvaluationMeasurementTechnique,
            command.SampleSize,
            command.SampleFrequency,
            command.ControlMethod,
            command.ReactionPlan,
            command.UserKey);

        _itemRepository.Add(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapItem(plan, item, sharedStep);
    }

    public async Task<ControlPlanItemResult> UpdateItemAsync(
        Guid itemId,
        SaveControlPlanItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanForProjectAsync(
            command.ProjectId, cancellationToken);

        plan.EnsureEditable();

        var item = await _itemRepository.GetByIdAsync(
            itemId, cancellationToken);

        if (item is null || item.ControlPlanId != plan.Id)
            throw new NotFoundException(
                $"Control Plan item '{itemId}' was not found in this project.");

        var sharedStep = await GetSharedStepForProjectAsync(
            command.ProjectId,
            command.ProductProcessStepId,
            cancellationToken);

        item.Update(
            sharedStep,
            command.CharacteristicNumber,
            command.CharacteristicType,
            command.CharacteristicName,
            command.MachineTooling,
            command.SpecialCharacteristic,
            command.SpecificationTolerance,
            command.EvaluationMeasurementTechnique,
            command.SampleSize,
            command.SampleFrequency,
            command.ControlMethod,
            command.ReactionPlan,
            command.UserKey);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapItem(plan, item, sharedStep);
    }

    public async Task<ControlPlanWorkflowResult> SubmitAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanForProjectAsync(
            projectId, cancellationToken);

        plan.EnsureEditable();

        var items = await _itemRepository.ListByControlPlanIdAsync(
            plan.Id, cancellationToken);

        if (items.Count == 0)
            throw new ConflictException(
                "A Control Plan must contain at least one control characteristic before review.");

        foreach (var item in items)
        {
            var sharedStep = await GetSharedStepForProjectAsync(
                projectId,
                item.ProductProcessStepId,
                cancellationToken);

            item.RefreshProcessSnapshot(sharedStep);
        }

        plan.SubmitForReview();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWorkflow(plan);
    }

    public async Task<ControlPlanWorkflowResult> ApproveAsync(
        Guid projectId,
        string approvedBy,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanForProjectAsync(
            projectId, cancellationToken);

        plan.Approve(approvedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWorkflow(plan);
    }

    public async Task<ControlPlanWorkflowResult> RejectAsync(
        RejectControlPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanForProjectAsync(
            command.ProjectId, cancellationToken);

        plan.Reject(command.Reason, command.UserKey);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWorkflow(plan);
    }

    private async Task<ControlPlan> GetPlanForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(projectId, cancellationToken);

        var plan = await _controlPlanRepository.GetByProjectIdAsync(
            projectId, cancellationToken);

        return plan ?? throw new NotFoundException(
            $"Control Plan for project '{projectId}' was not found.");
    }

    private async Task<Project> EnsureProjectExistsAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            projectId, cancellationToken);

        return project ?? throw new NotFoundException(
            $"Project '{projectId}' was not found.");
    }

    private async Task<ProductProcessStep> GetSharedStepForProjectAsync(
        Guid projectId,
        Guid sharedStepId,
        CancellationToken cancellationToken)
    {
        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                projectId, cancellationToken);

        if (sharedProcess is null)
            throw new ConflictException(
                "This project does not have an ASMF / Product Process.");

        var sharedStep = await _sharedStepRepository.GetByIdAsync(
            sharedStepId, cancellationToken);

        if (sharedStep is null ||
            sharedStep.ProductProcessId != sharedProcess.Id)
            throw new NotFoundException(
                $"Product Process Step '{sharedStepId}' does not belong to this project.");

        return sharedStep;
    }

    private async Task<ControlPlanWorkspaceResult> MapWorkspaceAsync(
        ControlPlan plan,
        CancellationToken cancellationToken)
    {
        var items = await _itemRepository.ListByControlPlanIdAsync(
            plan.Id, cancellationToken);

        var sharedProcess =
            await _productProcessRepository.GetByProjectIdAsync(
                plan.ProjectId, cancellationToken);

        var sharedById = new Dictionary<Guid, ProductProcessStep>();

        if (sharedProcess is not null)
        {
            var sharedSteps =
                await _sharedStepRepository.ListByProductProcessIdAsync(
                    sharedProcess.Id, cancellationToken);

            sharedById = sharedSteps.ToDictionary(step => step.Id);
        }

        var mappedItems = items
            .Select(item =>
            {
                sharedById.TryGetValue(
                    item.ProductProcessStepId,
                    out var sharedStep);

                return MapItem(plan, item, sharedStep);
            })
            .OrderBy(item => item.ProcessStepSequence)
            .ThenBy(item => item.CharacteristicNumber)
            .ThenBy(item => item.CharacteristicName)
            .ToList();

        return new ControlPlanWorkspaceResult(
            plan.Id,
            plan.ProjectId,
            plan.Number,
            plan.Name,
            plan.Status,
            plan.CreatedAt,
            plan.CreatedBy,
            plan.SubmittedAt,
            plan.ApprovedAt,
            plan.ApprovedBy,
            plan.RejectedAt,
            plan.RejectedBy,
            plan.RejectionReason,
            plan.IsEditable,
            mappedItems);
    }

    private static ControlPlanItemResult MapItem(
        ControlPlan plan,
        ControlPlanItem item,
        ProductProcessStep? sharedStep)
    {
        var useLiveShared =
            plan.IsEditable && sharedStep is not null;

        return new ControlPlanItemResult(
            item.Id,
            item.ControlPlanId,
            item.ProductProcessStepId,
            useLiveShared
                ? sharedStep!.Sequence
                : item.ProcessStepSequenceSnapshot,
            useLiveShared
                ? sharedStep!.Name
                : item.ProcessStepNameSnapshot,
            item.CharacteristicNumber,
            item.CharacteristicType,
            item.CharacteristicName,
            item.MachineTooling,
            item.SpecialCharacteristic,
            item.SpecificationTolerance,
            item.EvaluationMeasurementTechnique,
            item.SampleSize,
            item.SampleFrequency,
            item.ControlMethod,
            item.ReactionPlan,
            useLiveShared,
            useLiveShared ? "ASMF" : "Snapshot",
            item.CreatedAt,
            item.CreatedBy,
            item.UpdatedAt,
            item.UpdatedBy);
    }

    private static ControlPlanWorkflowResult MapWorkflow(
        ControlPlan plan) =>
        new(
            plan.Id,
            plan.ProjectId,
            plan.Status,
            plan.SubmittedAt,
            plan.ApprovedAt,
            plan.ApprovedBy,
            plan.RejectedAt,
            plan.RejectedBy,
            plan.RejectionReason);
}
