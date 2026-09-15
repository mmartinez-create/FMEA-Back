# Semana 2 - Día 7: Application

Este patch agrega los casos de uso de aplicación para la estructura PFMEA creada en el Día 6.

## Casos de uso
- CreateFmea
  - valida que el Project exista
  - rechaza proyectos archivados
  - evita números FMEA duplicados dentro del mismo proyecto
  - crea automáticamente Revision 01 en Draft
  - persiste FMEA + Revision en una sola unidad de trabajo
- CreateFmeaRevision
  - valida FMEA y revisión base
  - la revisión base debe pertenecer al mismo FMEA
  - reutiliza la regla de Domain: solo una revisión Approved puede originar la siguiente
- CreateProcessStep
  - solo permite editar revisiones Draft o Rejected
  - evita Sequence duplicada dentro de la misma revisión
  - persiste el ProcessStep

## Nuevos contratos de persistencia
- IFmeaRepository
- IFmeaRevisionRepository
- IProcessStepRepository

## Importante
Este día NO modifica EF Core ni el esquema SQL Server.
NO crear migration todavía.

Después de copiar el patch:
1. Compilar solución.
2. Ejecutar `dotnet test`.
3. Si todo pasa, continuar con Día 8: Infrastructure + migration `AddFmeaStructure`.
