using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Infrastructure.Persistence;
using FmeaManager.Infrastructure.Persistence.Repositories;
using FmeaManager.Infrastructure.Persistence.Revisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FmeaManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FmeaManagerDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'FmeaManagerDatabase' was not found.");

        services.AddDbContext<FmeaManagerDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();

        services.AddScoped<IFmeaRepository, FmeaRepository>();
        services.AddScoped<IFmeaRevisionRepository, FmeaRevisionRepository>();
        services.AddScoped<IFmeaRevisionContentCloner, FmeaRevisionContentCloner>();
        services.AddScoped<IProcessStepRepository, ProcessStepRepository>();

        services.AddScoped<IFailureModeRepository, FailureModeRepository>();
        services.AddScoped<IFailureEffectRepository, FailureEffectRepository>();
        services.AddScoped<IFailureCauseRepository, FailureCauseRepository>();
        services.AddScoped<IPreventionControlRepository, PreventionControlRepository>();
        services.AddScoped<IDetectionControlRepository, DetectionControlRepository>();
        services.AddScoped<IRiskAssessmentRepository, RiskAssessmentRepository>();

        services.AddScoped<IPlantRepository, PlantRepository>();
        services.AddScoped<IProductionLineRepository, ProductionLineRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPermissionAssignmentRepository, PermissionAssignmentRepository>();
        services.AddScoped<IProjectAuditRepository, ProjectAuditRepository>();

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<FmeaManagerDbContext>());


        services.AddScoped<IProductProcessRepository, ProductProcessRepository>();
        services.AddScoped<IProductProcessStepRepository, ProductProcessStepRepository>();
        services.AddScoped<IPfmeaProcessSyncRepository, PfmeaProcessSyncRepository>();
        services.AddScoped<IControlPlanRepository, ControlPlanRepository>();
        services.AddScoped<IControlPlanItemRepository, ControlPlanItemRepository>();

        return services;
    }
}
