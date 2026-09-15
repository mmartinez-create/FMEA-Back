# Día 23 — PFMEA Revision Management

## Objetivo

Completar el workflow real de revisiones PFMEA que faltaba durante la estabilización.

## Reglas implementadas

- Una revisión nueva sólo puede crearse desde la última revisión `Approved`.
- Sólo puede existir una revisión abierta por FMEA (`Draft`, `Under Review` o `Rejected`).
- La nueva revisión nace como `Draft`.
- El contenido de la revisión aprobada se copia a la nueva revisión:
  - Process Steps;
  - Failure Modes;
  - Failure Effects;
  - Failure Causes;
  - Prevention Controls;
  - Detection Controls;
  - Risk Assessments.
- Los `ProductProcessStepId` se conservan al copiar Process Steps.
  Por eso un Draft nuevo puede leer inmediatamente el valor vivo actual de ASMF,
  mientras la revisión Approved anterior conserva su snapshot.
- Cuando la revisión siguiente es aprobada, la revisión Approved anterior cambia a `Superseded`.

## Base de datos

No hay cambios de esquema y no se requiere migración.

## API

Se mantiene el contrato existente:

`POST /api/fmeas/{fmeaId}/revisions`

```json
{
  "basedOnRevisionId": "GUID",
  "revisionReason": "Process flow updated"
}
```

## Escenario E2E

1. Revision 01 Approved contiene `20 Assembly`.
2. ASMF cambia a `20 Final Assembly`.
3. Crear Revision 02 desde Revision 01.
4. Revision 02 Draft conserva el análisis anterior y muestra `20 Final Assembly` como dato vivo de ASMF.
5. Revision 01 Approved conserva `20 Assembly` como snapshot.
6. Al aprobar Revision 02, Revision 01 pasa a `Superseded`.
