# Semana 3 - Día 13: Failure Analysis REST API

## Objetivo

Exponer por HTTP el análisis de fallas construido en los Días 11 y 12.

## Endpoints

### Failure Modes
- GET  `/api/process-steps/{processStepId}/failure-modes`
- POST `/api/process-steps/{processStepId}/failure-modes`

### Failure Effects
- GET  `/api/failure-modes/{failureModeId}/effects`
- POST `/api/failure-modes/{failureModeId}/effects`

### Failure Causes
- GET  `/api/failure-modes/{failureModeId}/causes`
- POST `/api/failure-modes/{failureModeId}/causes`

### Prevention Controls
- GET  `/api/failure-causes/{failureCauseId}/prevention-controls`
- POST `/api/failure-causes/{failureCauseId}/prevention-controls`

### Detection Controls
- GET  `/api/failure-causes/{failureCauseId}/detection-controls`
- POST `/api/failure-causes/{failureCauseId}/detection-controls`

## Flujo completo disponible por API

Project
  -> PFMEA
     -> Revision
        -> Process Step
           -> Failure Mode
              -> Failure Effect
              -> Failure Cause
                 -> Prevention Control
                 -> Detection Control

## Reglas respetadas

Los POST reutilizan `FmeaEditabilityGuard`.

Por lo tanto, el análisis de fallas solo puede modificarse cuando la revisión está:
- Draft
- Rejected

Una revisión Under Review, Approved o Superseded no se puede modificar.

Los GET validan que el recurso padre exista. Si no existe, la API devuelve 404.

## Base de datos

Día 13 NO modifica el modelo de datos.

No se debe:
- crear migración;
- ejecutar `migrations add`;
- borrar la base de datos.

La migración más reciente continúa siendo `AddFailureAnalysis`.

## Validación local

Desde:
`C:\Users\maria.martinez\source\repos\FMEA-Api`

ejecutar:

1. `dotnet restore`
2. `dotnet build`
3. `dotnet test`
4. `dotnet run --project .\FMEA-Api\FMEA-Api.csproj`

Después se pueden probar los endpoints desde `FMEA-Api.http`.

## Siguiente incremento

Día 14:
Angular PFMEA Workspace.

La interfaz dejará de presentar únicamente Process Steps y empezará a mostrar el análisis jerárquico:

Process Step
  -> Failure Mode
     -> Effect
     -> Cause
        -> Prevention Control
        -> Detection Control
