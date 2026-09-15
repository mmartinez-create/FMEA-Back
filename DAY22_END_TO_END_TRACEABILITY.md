# Día 22 — Trazabilidad y sincronización ASMF ↔ PFMEA ↔ Control Plan

## Objetivo

Cerrar el MVP solicitado en la junta con una cadena documental que comparte
la misma estructura de proceso y permite verificar su estado desde el Project.

## Fuente común

ProductProcess
  -> ProductProcessStep
     -> ASMF
     -> PFMEA ProcessStep.ProductProcessStepId
     -> ControlPlanItem.ProductProcessStepId

No se crean tres copias independientes de la estructura.

## Trazabilidad

Nuevo endpoint:

GET `/api/projects/{projectId}/document-traceability`

Devuelve:
- estado de ASMF;
- estado de PFMEA;
- estado de Control Plan;
- estado global;
- últimas revisiones PFMEA activas;
- conteos de Failure Modes y Failure Causes;
- causas con S/O/D;
- Action Priority H/M/L;
- características de Control Plan;
- matriz por ProductProcessStep.

## Sincronización

Nuevo endpoint:

POST `/api/projects/{projectId}/document-traceability/synchronize`

Con permiso Edit:
- sincroniza las últimas revisiones PFMEA que estén Draft o Rejected;
- crea vínculos faltantes desde ASMF;
- enlaza Process Steps legacy por Sequence cuando es seguro;
- refresca snapshots PFMEA;
- refresca snapshots del Control Plan si está Draft o Rejected.

Los documentos Under Review o Approved no se reescriben.

## Estados

Missing:
no existe el documento o la fuente requerida.

Ready:
la fuente existe y el documento está listo para completarse.

NeedsSync:
PFMEA todavía no contiene todos los ProductProcessStep compartidos.

Synchronized:
la relación estructural está consistente.

Attention:
hay una referencia que ya no corresponde a la estructura ASMF actual.

## Inmutabilidad

PFMEA y Control Plan continúan leyendo datos vivos de ASMF mientras están
editables. Al entrar a revisión conservan su snapshot, evitando modificar
retroactivamente documentos sometidos o aprobados.

## Auditoría

La sincronización global genera:
`ASMF, PFMEA and Control Plan synchronized`

Last Activity sigue mostrando únicamente el último evento, mientras la base
conserva el historial completo.

## Migración

Día 22 NO agrega tablas ni columnas.

No ejecutar `dotnet ef migrations add` por Día 22.

La funcionalidad utiliza las relaciones persistidas en Día 19 y Día 21.

## Resultado del MVP

Plant -> Production Line -> Product -> Project
                                   -> ASMF
                                   -> PFMEA AIAG/VDA
                                   -> Control Plan AIAG

Con:
- permisos Read/Edit/Approve;
- Last Activity;
- workflow;
- snapshots;
- Action Priority;
- trazabilidad por Process Step;
- sincronización documental.
