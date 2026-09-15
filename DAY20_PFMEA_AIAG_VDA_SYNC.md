# Día 20 — PFMEA AIAG/VDA + sincronización con ASMF

## Objetivo

Conectar el PFMEA al ProductProcess compartido introducido en Día 19.

El dato estructural del proceso ya no se mantiene como una copia
independiente en el flujo normal.

## Fuente compartida

Project
  -> ProductProcess
     -> ProductProcessStep
          -> ASMF
          -> PFMEA

Cada ProcessStep del PFMEA puede apuntar a:

`ProductProcessStepId`

Cuando la revisión está Draft o Rejected, el endpoint sincronizado
presenta Sequence, Name, Function y Requirement directamente desde
ProductProcessStep.

## Snapshot e inmutabilidad

Cuando una revisión se envía a Under Review:

1. el sistema sincroniza la estructura actual de ASMF;
2. refresca el snapshot almacenado en ProcessSteps;
3. cambia la revisión a Under Review.

Las revisiones Under Review, Approved y Superseded leen el snapshot,
no el valor vivo de ASMF.

Esto evita que una edición posterior en ASMF cambie retroactivamente
un PFMEA ya sometido o aprobado.

## Sincronización

Endpoint:

POST `/api/fmea-revisions/{revisionId}/sync-product-process`

Comportamiento:
- crea ProcessSteps faltantes;
- enlaza ProcessSteps legacy por Sequence cuando es seguro;
- refresca snapshots de ProcessSteps ya enlazados;
- detecta colisiones de Sequence.

Lectura sincronizada:

GET `/api/fmea-revisions/{revisionId}/synced-process-steps`

Cada fila indica:
- ProductProcessStepId;
- IsShared;
- IsLiveShared;
- Source = ASMF / Snapshot / Legacy.

## Action Priority

Día 20 agrega Action Priority:

- High
- Medium
- Low

La AP se calcula en Domain a partir de Severity, Occurrence y Detection.

RPN se conserva por compatibilidad y referencia, pero la UI presenta
Action Priority como el indicador principal de priorización.

ActionPriority es una propiedad calculada y NO se persiste en SQL.

## Migración

Día 20 NO cambia el modelo persistente.

No ejecutar:

`dotnet ef migrations add ...`

No ejecutar una migración nueva por Día 20.

El esquema requerido ya fue introducido en Día 19:
- ProductProcesses
- ProductProcessSteps
- ProcessSteps.ProductProcessStepId

## Base local

Los archivos:
- appsettings.json
- appsettings.Development.json

incluyen la cadena:

`FmeaManagerDatabase`

apuntando a:

`FmeaManagerDbV2`

## Siguiente incremento

Día 21:
Control Plan AIAG sobre los mismos ProductProcessStep.

La cadena quedará:

ASMF -> PFMEA -> Control Plan
