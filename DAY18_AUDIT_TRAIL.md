# Día 18 — Project Audit Trail + Last Activity

## Requisito de la junta

Guardar el historial de cambios del proyecto con:
- nombre del evento;
- fecha/hora;
- usuario que realizó el cambio;

pero mostrar en la interfaz únicamente el evento más reciente.

## Diseño implementado

La base conserva todos los eventos en:
`ProjectAuditEvents`

Cada registro contiene:
- ProjectId
- EventName
- Action
- EntityType
- EntityId
- ActorUserKey
- ActorDisplayName
- OccurredAt

La UI NO lista el historial completo.
Solo consulta:

`GET /api/projects/{projectId}/last-activity`

El backend devuelve el evento más reciente por `OccurredAt DESC`.

## Eventos auditados en Día 18

- Project created
- PFMEA created
- PFMEA revision created
- Process step added
- Failure mode added
- Failure effect added
- Failure cause added
- Prevention control added
- Detection control added
- Risk assessment updated
- PFMEA revision submitted for review
- PFMEA revision approved
- PFMEA revision rejected

## Permisos

El endpoint Last Activity requiere permiso Read sobre el proyecto.

Los eventos se generan únicamente después de que la operación principal
ha sido autorizada por Read/Edit/Approve.

## Identidad

Actualmente:
`X-Fmea-User`

se usa como identificador del actor.

`ActorDisplayName` queda preparado para tomar el claim `name`
cuando se conecte Entra ID / Azure AD.

## Migración

El ZIP de Día 18 NO reemplaza la carpeta Migrations.

Esto es intencional porque la historia de migraciones se corrigió
localmente con EF Core.

Después de extraer Día 18, generar localmente:

`dotnet ef migrations add AddProjectAuditTrail --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj --output-dir Persistence\Migrations`

y luego:

`dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

## Siguiente incremento

Día 19:
ProductProcess / ProcessStep común + ASMF.

Ese será el primer paso para que:
ASMF -> PFMEA -> Control Plan

usen la misma estructura en lugar de copiar información.
