using FmeaManager.Application.DocumentTraceability;
using FmeaManager.Application.ControlPlans;
using FmeaManager.Application.Fmeas.SyncProductProcess;
using FmeaManager.Application.Fmeas.GetSyncedProcessSteps;
using FmeaManager.Application.ProductProcesses.Create;
using FmeaManager.Application.ProductProcesses.CreateStep;
using FmeaManager.Application.ProductProcesses.GetByProject;
using FmeaManager.Application.ProductProcesses.UpdateStep;
using FmeaManager.Application.Auditing;
using FmeaManager.Application.AccessControl.RevokePermissionAssignment;
using FmeaManager.Application.AccessControl;
using FmeaManager.Application.AccessControl.GetUserAssignments;
using FmeaManager.Application.AccessControl.UpsertPermissionAssignment;
using FmeaManager.Application.Fmeas.ChangeRevisionStatus;
using FmeaManager.Application.ProductStructure.CreatePlant;
using FmeaManager.Application.ProductStructure.CreateProductionLine;
using FmeaManager.Application.ProductStructure.CreateProduct;
using FmeaManager.Application.ProductStructure.CreateProjectForProduct;
using FmeaManager.Application.ProductStructure.GetProductStructure;
using FmeaManager.Application.Fmeas.UpsertRiskAssessment;
using FmeaManager.Application.Fmeas.GetRiskAssessment;
using FmeaManager.Application.Fmeas.GetDetectionControlsByFailureCause;
using FmeaManager.Application.Fmeas.GetFailureCausesByFailureMode;
using FmeaManager.Application.Fmeas.GetFailureEffectsByFailureMode;
using FmeaManager.Application.Fmeas.GetFailureModesByProcessStep;
using FmeaManager.Application.Fmeas.GetPreventionControlsByFailureCause;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Application.Fmeas.CreateDetectionControl;
using FmeaManager.Application.Fmeas.CreateFailureCause;
using FmeaManager.Application.Fmeas.CreateFailureEffect;
using FmeaManager.Application.Fmeas.CreateFailureMode;
using FmeaManager.Application.Fmeas.CreatePreventionControl;
using FmeaManager.Application.CustomerProfiles.GetCustomerProfiles;
using FmeaManager.Application.Fmeas.CreateFmea;
using FmeaManager.Application.Fmeas.CreateProcessStep;
using FmeaManager.Application.Fmeas.CreateRevision;
using FmeaManager.Application.Fmeas.GetFmeaById;
using FmeaManager.Application.Fmeas.GetFmeasByProject;
using FmeaManager.Application.Fmeas.GetProcessStepsByRevision;
using FmeaManager.Application.Fmeas.GetRevisionsByFmea;
using FmeaManager.Application.Projects.CreateProject;
using FmeaManager.Application.Projects.GetProjectById;
using FmeaManager.Application.Projects.GetProjects;
using Microsoft.Extensions.DependencyInjection;

namespace FmeaManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProjectHandler>();
        services.AddScoped<GetProjectsHandler>();
        services.AddScoped<GetProjectByIdHandler>();
        services.AddScoped<GetCustomerProfilesHandler>();

        services.AddScoped<CreateFmeaHandler>();
        services.AddScoped<GetFmeasByProjectHandler>();
        services.AddScoped<GetFmeaByIdHandler>();

        services.AddScoped<CreateFmeaRevisionHandler>();
        services.AddScoped<GetFmeaRevisionsHandler>();

        services.AddScoped<CreateProcessStepHandler>();
        services.AddScoped<GetProcessStepsHandler>();
        services.AddScoped<GetSyncedProcessStepsHandler>();
        services.AddScoped<PfmeaProcessSynchronizer>();
        services.AddScoped<SyncProductProcessHandler>();

        services.AddScoped<FmeaEditabilityGuard>();
        services.AddScoped<CreateFailureModeHandler>();
        services.AddScoped<CreateFailureEffectHandler>();
        services.AddScoped<CreateFailureCauseHandler>();
        services.AddScoped<CreatePreventionControlHandler>();
        services.AddScoped<CreateDetectionControlHandler>();

        services.AddScoped<GetFailureModesByProcessStepHandler>();
        services.AddScoped<GetFailureEffectsByFailureModeHandler>();
        services.AddScoped<GetFailureCausesByFailureModeHandler>();
        services.AddScoped<GetPreventionControlsByFailureCauseHandler>();
        services.AddScoped<GetDetectionControlsByFailureCauseHandler>();

        services.AddScoped<GetRiskAssessmentHandler>();
        services.AddScoped<UpsertRiskAssessmentHandler>();

        services.AddScoped<CreatePlantHandler>();
        services.AddScoped<CreateProductionLineHandler>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<CreateProjectForProductHandler>();
        services.AddScoped<GetProductStructureHandler>();

        services.AddScoped<AccessControlService>();
        services.AddScoped<ProjectAuditService>();
        services.AddScoped<GetLastProjectActivityHandler>();
        services.AddScoped<UpsertPermissionAssignmentHandler>();
        services.AddScoped<GetUserAssignmentsHandler>();
        services.AddScoped<RevokePermissionAssignmentHandler>();

        services.AddScoped<SubmitRevisionForReviewHandler>();
        services.AddScoped<ApproveRevisionHandler>();
        services.AddScoped<RejectRevisionHandler>();


        services.AddScoped<GetProductProcessByProjectHandler>();
        services.AddScoped<CreateProductProcessHandler>();
        services.AddScoped<CreateProductProcessStepHandler>();
        services.AddScoped<UpdateProductProcessStepHandler>();
        services.AddScoped<ControlPlanService>();
        services.AddScoped<ProjectDocumentTraceabilityService>();

        return services;
    }
}
