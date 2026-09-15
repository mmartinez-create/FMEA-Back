# Día 19 — Shared Product Process + ASMF

## Objetivo

Crear el primer modelo realmente compartido entre:

ASMF -> PFMEA -> Control Plan

La información base del proceso ya no debe copiarse tres veces.

## Modelo

Project
  -> ProductProcess (1:1)
     -> ProductProcessStep (1:N)

Cada ProductProcessStep contiene:
- Sequence
- Name
- Function
- Requirement

Estas propiedades serán la fuente común que utilizarán las tres vistas.

## ASMF

El frontend incorpora un nuevo workspace:

`/projects/{projectId}/asmf`

La pantalla representa visualmente el flujo:

10 -> 20 -> 30 -> ...

y permite crear y editar Process Steps.

## Puente hacia PFMEA

El ProcessStep histórico del PFMEA ahora incluye:

`ProductProcessStepId`

nullable.

Esto mantiene compatibilidad con los PFMEA existentes y prepara Día 20,
donde el PFMEA comenzará a consumir los Process Steps compartidos.

En Día 19 NO se duplica automáticamente cada ASMF step al PFMEA.
La vinculación completa se realiza en Día 20.

## Permisos

ASMF respeta el control de Día 17:

Read:
- visualizar Product Process y ASMF.

Edit:
- crear Product Process;
- agregar Process Steps;
- modificar Process Steps.

## Auditoría

Los cambios generan eventos:

- ASMF product process created
- ASMF process step added
- ASMF process step edited

Por lo tanto el panel Last Activity de Día 18 se actualiza también con ASMF.

## API

GET  `/api/projects/{projectId}/product-process`
POST `/api/projects/{projectId}/product-process`
POST `/api/product-processes/{productProcessId}/steps`
PUT  `/api/product-process-steps/{stepId}`

## Migración

El ZIP NO reemplaza tu carpeta Migrations.

Después de extraer Día 19 y comprobar build/test:

`dotnet ef migrations add AddSharedProductProcess --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj --output-dir Persistence\Migrations`

Después:

`dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

Esta migración debe crear:
- ProductProcesses
- ProductProcessSteps

y agregar:
- ProcessSteps.ProductProcessStepId

## Siguiente incremento

Día 20:
PFMEA AIAG/VDA sobre el ProductProcess compartido.

El objetivo será que al modificar un dato estructural del proceso en ASMF,
el PFMEA vea el mismo dato porque ambos apuntan al mismo ProductProcessStep.
