# Día 21 — Control Plan AIAG

## Implementado

Project -> ControlPlan -> ControlPlanItem -> ProductProcessStep

Cada característica del Control Plan queda ligada al mismo Process Step
que utiliza ASMF y que PFMEA ya puede consumir.

Campos principales:
- Characteristic Number
- Product / Process Characteristic
- Characteristic Name
- Machine / Tooling
- Special Characteristic
- Specification / Tolerance
- Evaluation / Measurement Technique
- Sample Size
- Sample Frequency
- Control Method
- Reaction Plan

## Sincronización e inmutabilidad

Draft / Rejected:
- Process Step Sequence y Name se leen vivos desde ASMF.

Submit for Review:
- refresca el snapshot del Process Step.

Under Review / Approved:
- lee el snapshot congelado.

## Workflow

Draft / Rejected -> Submit [Edit]
Under Review -> Approve / Reject [Approve]
Approved -> Read-only

## Migración

La versión consolidada corregida de Día 22 incluye Control Plan dentro de
la migración limpia `InitialDay22`. No generes `AddControlPlan` nuevamente.

En Development el backend aplica automáticamente la migración contra la
base separada `FmeaManagerDbDay22`.

## Día 22

Cerrará la trazabilidad ASMF ↔ PFMEA ↔ Control Plan y el estado de
sincronización del proyecto.
