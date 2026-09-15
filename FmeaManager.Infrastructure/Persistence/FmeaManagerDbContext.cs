using FmeaManager.Domain.ControlPlans;
using FmeaManager.Domain.Auditing;
using FmeaManager.Domain.AccessControl;
using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Domain.CustomerProfiles;
using FmeaManager.Domain.Fmeas;
using FmeaManager.Domain.Projects;
using FmeaManager.Domain.ProductStructure;
using Microsoft.EntityFrameworkCore;

namespace FmeaManager.Infrastructure.Persistence;

public sealed class FmeaManagerDbContext : DbContext, IUnitOfWork
{
    public FmeaManagerDbContext(DbContextOptions<FmeaManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    public DbSet<PermissionAssignment> PermissionAssignments => Set<PermissionAssignment>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectAuditEvent> ProjectAuditEvents => Set<ProjectAuditEvent>();

    public DbSet<ControlPlan> ControlPlans => Set<ControlPlan>();

    public DbSet<ControlPlanItem> ControlPlanItems => Set<ControlPlanItem>();


    public DbSet<Plant> Plants => Set<Plant>();

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductProcess> ProductProcesses => Set<ProductProcess>();

    public DbSet<ProductProcessStep> ProductProcessSteps => Set<ProductProcessStep>();

    public DbSet<Fmea> Fmeas => Set<Fmea>();

    public DbSet<FmeaRevision> FmeaRevisions => Set<FmeaRevision>();

    public DbSet<ProcessStep> ProcessSteps => Set<ProcessStep>();


    public DbSet<FailureMode> FailureModes => Set<FailureMode>();

    public DbSet<FailureEffect> FailureEffects => Set<FailureEffect>();

    public DbSet<FailureCause> FailureCauses => Set<FailureCause>();

    public DbSet<PreventionControl> PreventionControls => Set<PreventionControl>();

    public DbSet<DetectionControl> DetectionControls => Set<DetectionControl>();

    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FmeaManagerDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
