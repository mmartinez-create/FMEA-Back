# Semana 2 - Día 9: API REST de estructura PFMEA

## Objetivo
Conectar la estructura FMEA construida en Domain/Application/Infrastructure con HTTP.
No se modifica el esquema de base de datos, por lo que Día 9 no requiere una migración nueva.

## Endpoints

### PFMEA/FMEA por proyecto
- `GET /api/projects/{projectId}/fmeas`
- `POST /api/projects/{projectId}/fmeas`

Al crear un FMEA se crea también su Revision 01 en estado Draft dentro de la misma unidad de trabajo.

### FMEA
- `GET /api/fmeas/{fmeaId}`

### Revisiones
- `GET /api/fmeas/{fmeaId}/revisions`
- `POST /api/fmeas/{fmeaId}/revisions`

La creación de una siguiente revisión conserva la regla de dominio existente: solo puede partir de una revisión Approved.

### Process Steps
- `GET /api/fmea-revisions/{revisionId}/process-steps`
- `POST /api/fmea-revisions/{revisionId}/process-steps`

Solo una revisión Draft o Rejected admite nuevos Process Steps.

## Application
Se añadieron cuatro queries/handlers de lectura:
- `GetFmeasByProjectHandler`
- `GetFmeaByIdHandler`
- `GetFmeaRevisionsHandler`
- `GetProcessStepsHandler`

Cada handler valida la existencia del recurso padre antes de devolver colecciones. Esto evita que una URL con un Project/FMEA/Revision inexistente responda engañosamente con una lista vacía.

## API
Se añadieron contratos HTTP separados de las entidades de dominio:
- `CreateFmeaRequest`
- `CreateFmeaRevisionRequest`
- `CreateProcessStepRequest`

Se añadieron:
- `FmeasController`
- `FmeaRevisionsController`
- endpoints FMEA anidados en `ProjectsController`

`CreatedBy` continúa siendo controlado por servidor (`User.Identity.Name` o `local-development`) y no viene del request.

## Pruebas
Se añadieron tests de Application para los cuatro handlers de lectura, incluyendo escenarios NotFound.

## Persistencia
Se conserva íntegra la migración local generada al cierre de Día 8:
`20260909230944_InitialCreate`.

No ejecutar `dotnet ef migrations add` para Día 9.
