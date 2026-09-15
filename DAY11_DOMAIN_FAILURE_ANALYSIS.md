# Semana 3 - Día 11: Failure Analysis Domain

## Objetivo
Agregar al dominio los conceptos fundamentales del análisis de fallas de un PFMEA.

## Modelo agregado

ProcessStep
  -> FailureMode
       -> FailureEffect
       -> FailureCause
            -> PreventionControl
            -> DetectionControl

## Entidades

### FailureMode
Representa la forma en que una operación puede fallar.

Campos:
- Id
- ProcessStepId
- Description

### FailureEffect
Representa la consecuencia del Failure Mode.

Campos:
- Id
- FailureModeId
- Description

La calificación Severity se agregará en el incremento de Risk Assessment.

### FailureCause
Representa la causa potencial del Failure Mode.

Campos:
- Id
- FailureModeId
- Description

La calificación Occurrence se agregará en Risk Assessment.

### PreventionControl
Control actual orientado a prevenir una causa.

Campos:
- Id
- FailureCauseId
- Description

### DetectionControl
Control actual orientado a detectar la causa/falla.

Campos:
- Id
- FailureCauseId
- Description

La calificación Detection se agregará en Risk Assessment.

## Reglas de dominio implementadas

- Todas las entidades requieren un parent Id válido.
- Las descripciones son obligatorias.
- Las descripciones se normalizan con Trim().
- Los Ids se generan dentro de las factorías Create().
- Las propiedades usan private set.
- EF Core podrá usar los constructores privados en el siguiente incremento.
- Cada entidad permite modificar únicamente su descripción mediante comportamiento explícito.

## Importante

Este día modifica únicamente Domain + Domain.Tests.

NO:
- modifica el DbContext;
- modifica SQL Server;
- requiere migración;
- agrega endpoints;
- agrega Angular.

Eso se hará incrementalmente en los siguientes días.

## Siguiente incremento

Día 12:
- interfaces de repositorio;
- handlers Application;
- implementaciones Infrastructure;
- EF Core configurations;
- DbSet;
- migración AddFailureAnalysis.
