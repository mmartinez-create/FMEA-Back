# Semana 3 - Día 15: Risk Assessment + RPN

## Objetivo

Agregar la primera evaluación cuantitativa de riesgo al PFMEA.

## Modelo

FailureCause
  -> RiskAssessment
       - Severity (1..10)
       - Occurrence (1..10)
       - Detection (1..10)
       - RPN = S x O x D

Se mantiene un único RiskAssessment vigente por FailureCause.

## Por qué el RiskAssessment cuelga de FailureCause

Occurrence y Detection dependen directamente de una causa y sus controles.
Esto también deja una unidad clara para futuras Corrective Actions.

Severity se mantiene dentro de la misma evaluación MVP para calcular el RPN
sin duplicar lógica en Angular. En una evolución posterior, cuando se
introduzcan RatingScale configurables y reglas AIAG/VDA/Ford, la estrategia
de scoring podrá especializarse sin cambiar la jerarquía principal.

## Reglas

- S, O y D deben estar entre 1 y 10.
- RPN nunca se recibe del cliente.
- El dominio calcula `RPN = Severity * Occurrence * Detection`.
- SQL Server tiene CHECK constraints para 1..10 y para la consistencia del RPN.
- Solo existe un RiskAssessment por FailureCause.
- El score solo puede modificarse si la FMEA Revision está Draft o Rejected.
- Under Review, Approved y Superseded permanecen read-only.
- Se guarda CreatedAt/CreatedBy y UpdatedAt/UpdatedBy.

## API

GET
`/api/failure-causes/{failureCauseId}/risk-assessment`

- 200 cuando existe score.
- 204 cuando la causa existe pero aún no tiene score.
- 404 cuando la FailureCause no existe.

PUT
`/api/failure-causes/{failureCauseId}/risk-assessment`

Body:

```json
{
  "severity": 9,
  "occurrence": 4,
  "detection": 3
}
```

Respuesta:

```json
{
  "severity": 9,
  "occurrence": 4,
  "detection": 3,
  "rpn": 108
}
```

## Migración

Incluida:
`AddRiskAssessment`

NO ejecutar `dotnet ef migrations add`.

Desde:
`C:\Users\maria.martinez\source\repos\FMEA-Api`

ejecutar:

1. `dotnet restore`
2. `dotnet build`
3. `dotnet test`
4. `dotnet ef migrations list --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`
5. `dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

Después debe existir la tabla:
`RiskAssessments`

## Importante sobre RPN

En este MVP RPN es el método inicial de priorización.
No se están declarando umbrales RPN como criterio oficial de aprobación
ni como regla Ford/AIAG-VDA.

El modelo seguirá siendo extensible para agregar Action Priority y
RatingScale configurables.
