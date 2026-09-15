# Semana 2 - Día 8: Infrastructure + EF Core

Este patch resuelve el error de Dependency Injection:

- IFmeaRepository
- IFmeaRevisionRepository
- IProcessStepRepository

Ahora cada interfaz tiene una implementación real en Infrastructure y queda
registrada en el contenedor de dependencias.

## Cambios
- DbSet<Fmea>
- DbSet<FmeaRevision>
- DbSet<ProcessStep>
- FmeaConfiguration
- FmeaRevisionConfiguration
- ProcessStepConfiguration
- FmeaRepository
- FmeaRevisionRepository
- ProcessStepRepository
- registros DI

## Índices / constraints
- UNIQUE (ProjectId, Number) en Fmeas
- UNIQUE (FmeaId, RevisionNumber) en FmeaRevisions
- UNIQUE (FmeaRevisionId, Sequence) en ProcessSteps
- FK Fmeas -> Projects
- FK FmeaRevisions -> Fmeas
- FK FmeaRevisions -> FmeaRevisions (BasedOnRevisionId)
- FK ProcessSteps -> FmeaRevisions

## Después de aplicar
1. `dotnet clean`
2. `dotnet test`
3. Ejecutar la API: ya no debe fallar en `builder.Build()`.
4. Crear migration:
   `dotnet ef migrations add AddFmeaStructure --project FmeaManager.Infrastructure --startup-project FMEA-Api --output-dir Persistence/Migrations`
5. Aplicarla:
   `dotnet ef database update --project FmeaManager.Infrastructure --startup-project FMEA-Api`

NO vuelvas a crear InitialCreate.
Esta es la segunda migración de la misma base de datos.
