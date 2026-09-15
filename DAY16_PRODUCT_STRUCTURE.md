# Día 16 — Product Structure

Este incremento reorienta el MVP al alcance definido en la junta:

Plant
  -> Production Line
     -> Product
        -> Project
           -> ASMF / Process Flow
           -> PFMEA AIAG / VDA
           -> Control Plan AIAG

## Implementado hoy

### Modelo central
- Plant
- ProductionLine
- Product
- Project.ProductId (nullable por compatibilidad con proyectos existentes)

### API
- GET  `/api/product-structure`
- POST `/api/plants`
- POST `/api/plants/{plantId}/production-lines`
- POST `/api/production-lines/{productionLineId}/products`
- POST `/api/products/{productId}/projects`

### Compatibilidad
El endpoint legado `POST /api/projects` continúa funcionando.
Los proyectos previos permanecen válidos aunque ProductId sea null.

Los proyectos creados desde Product Structure sí quedan ligados
estructuralmente a Product -> ProductionLine -> Plant.

### Sincronización futura
ASMF, PFMEA y Control Plan NO se modelarán como tres copias independientes.
Los tres consumirán el mismo ProductProcess / ProcessStep.

Ese trabajo comienza en Día 19.

## Migración
Incluida:
`AddProductStructure`

No ejecutar `dotnet ef migrations add`.

Aplicar:
`dotnet ef database update --project .\FmeaManager.Infrastructure\FmeaManager.Infrastructure.csproj --startup-project .\FMEA-Api\FMEA-Api.csproj`

## Siguiente incremento
Día 17:
- Read
- Edit
- Approve
- scope heredable por Plant / Production Line / Product / Project
