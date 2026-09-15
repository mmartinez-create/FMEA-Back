# Día 17 — Access Control: Read / Edit / Approve

## Objetivo

Implementar el control de acceso solicitado en la junta sobre la jerarquía:

System
  -> Plant
     -> Production Line
        -> Product
           -> Project
              -> PFMEA / Revision / Process / Failure Analysis

## Permisos

- Read
- Edit
- Approve

`Edit` agrega implícitamente `Read`.
`Approve` agrega implícitamente `Read`, pero NO agrega `Edit`.

Esto permite un aprobador que pueda revisar y aprobar sin modificar el contenido.

## Herencia

Los permisos se acumulan desde los ancestros.

Ejemplo:

Usuario A:
- Plant MX-TOL: Read + Edit

Entonces obtiene Read + Edit en:
- todas las Production Lines de MX-TOL;
- todos los Products de esas líneas;
- todos los Projects ligados a esos Products;
- PFMEAs y revisiones dentro de esos Projects.

Un permiso directo en un nivel inferior puede AGREGAR capacidades.

Ejemplo:
- Plant MX-TOL: Read
- Product BAT-001: Edit
- Project PROJ-BAT-001: Approve

Resultado en PROJ-BAT-001:
- Read
- Edit
- Approve

En esta versión no se implementan reglas de "deny".
La herencia solo agrega permisos.

## Scope Types

0 = System
1 = Plant
2 = ProductionLine
3 = Product
4 = Project

System usa:
`00000000-0000-0000-0000-000000000000`

## Identidad temporal

Todavía no existe autenticación corporativa.

Para poder probar autorización real, Angular envía:

`X-Fmea-User`

Ejemplo:

`X-Fmea-User: quality.engineer@company.com`

La identidad por defecto es:

`local-development`

La migración de Día 17 debe insertar a `local-development`
con Read + Edit + Approve a nivel System para evitar bloquear
el ambiente local.

Cuando se conecte Entra ID / Azure AD este header temporal
debe eliminarse y reemplazarse por la identidad autenticada.

## Enforcement

La API ya valida permisos.

READ:
- Product Structure visible
- Project
- PFMEA
- Revisions
- Process Steps
- Failure Modes / Effects / Causes
- Controls
- Risk Assessment

EDIT:
- Crear Plant (System Edit)
- Crear Production Line (Plant Edit)
- Crear Product (ProductionLine Edit)
- Crear Project (Product Edit)
- Crear PFMEA
- Crear Revision
- Crear Process Step
- Crear Failure Analysis
- Modificar Risk Assessment

APPROVE:
- Gestionar asignaciones de permisos en un scope
- Aprobar una revisión
- Rechazar una revisión

## Workflow de aprobación agregado

Draft
  -> Submit for Review  [Edit]
Under Review
  -> Approve            [Approve]
  -> Reject             [Approve]

Las reglas de estado del dominio siguen vigentes.

## API

GET `/api/access-control/me`

GET `/api/access-control/effective?scopeType={n}&scopeId={guid}`

GET `/api/access-control/users/{userKey}/assignments`

PUT `/api/access-control/users/{userKey}/assignments`

DELETE `/api/access-control/users/{userKey}/assignments?scopeType={n}&scopeId={guid}`

POST `/api/fmea-revisions/{revisionId}/submit`

POST `/api/fmea-revisions/{revisionId}/approve`

POST `/api/fmea-revisions/{revisionId}/reject`

## IMPORTANTE — migración en tu computadora

Después del problema anterior con `PendingModelChangesWarning`,
NO se entrega una migración EF fabricada manualmente.

El ZIP de Día 17 para CurrentPath NO reemplaza tu carpeta:

`FmeaManager.Infrastructure/Persistence/Migrations`

Conserva las migraciones que EF generó correctamente en tu equipo.

Después de extraer Día 17 sobre tu proyecto actual:

`dotnet build`
`dotnet test`

Luego genera SOLO la migración del nuevo modelo:

`dotnet ef migrations add AddAccessControl --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj --output-dir Persistence\Migrations`

Después:

`dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

Finalmente:

`dotnet ef migrations has-pending-model-changes --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

El resultado esperado es que no existan cambios pendientes.

## Siguiente incremento

Día 18:
Audit Trail + Last Activity.

Se guardará todo el historial, pero el dashboard del proyecto
mostrará solamente el último evento, tal como fue solicitado.
