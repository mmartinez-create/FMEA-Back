# Semana 3 - Día 12: Failure Analysis - Application + Infrastructure

## Incluido
- Interfaces de repositorio:
  - IFailureModeRepository
  - IFailureEffectRepository
  - IFailureCauseRepository
  - IPreventionControlRepository
  - IDetectionControlRepository
- FmeaEditabilityGuard para impedir cambios en revisiones no editables.
- Handlers de creación para las cinco entidades.
- Repositories EF Core.
- EntityTypeConfiguration para las cinco entidades.
- DbSet en FmeaManagerDbContext.
- Registro de DI en Application e Infrastructure.
- Migración `AddFailureAnalysis`.
- Snapshot de EF Core actualizado.

## Nuevas tablas
- FailureModes
- FailureEffects
- FailureCauses
- PreventionControls
- DetectionControls

## Relaciones
ProcessSteps 1:N FailureModes
FailureModes 1:N FailureEffects
FailureModes 1:N FailureCauses
FailureCauses 1:N PreventionControls
FailureCauses 1:N DetectionControls

## Regla importante
Solo se puede modificar el análisis de fallas cuando la FMEA Revision está en:
- Draft
- Rejected

UnderReview, Approved y Superseded quedan protegidas contra edición.

## En tu PC
1. `dotnet restore`
2. `dotnet build`
3. `dotnet test`
4. `dotnet ef migrations list --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`
5. `dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

Después de `database update`, SQL Server debe tener las cinco tablas nuevas.

## Siguiente día
Día 13: REST API para crear/consultar Failure Modes, Effects, Causes y Controls.
