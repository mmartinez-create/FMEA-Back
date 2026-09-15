# FMEA Manager — revisión profunda Backend + Frontend

## Problemas reales encontrados

1. El frontend enviaba `/api` a `https://localhost:7203`, pero el perfil HTTP
   por defecto del backend escucha en `http://localhost:5170`. La integración
   dependía de qué perfil de Visual Studio / `dotnet run` se hubiera iniciado.
2. `UseHttpsRedirection()` también estaba activo en Development, agregando otra
   fuente de redirecciones/puertos durante el uso del proxy Angular.
3. La única migración del backend se llamaba `AddControlPlan`, pero en realidad
   intentaba crear las 20 tablas del sistema. No era incremental y no era segura
   para una base V2 que ya tuviera CustomerProfiles, Projects, etc.
4. Project Detail cargaba Traceability dentro del mismo `forkJoin`; un fallo de
   ese endpoint impedía cargar el proyecto completo.
5. La interfaz decía `API connected` de forma estática aun cuando el backend
   estuviera apagado o la base incompleta.
6. El build de producción tenía un límite de 4 KB por CSS de componente, pero
   existen workspaces de hasta ~30 KB; `npm run build` podía fallar por budget.
7. El ZIP del backend contenía `bin`, `obj`, `.vs` y archivos de usuario de
   Visual Studio, mezclando artefactos locales con el código fuente.

## Correcciones aplicadas

- Contrato de desarrollo único:
  - Angular: `http://localhost:4200`
  - API: `http://localhost:5170`
  - `/api` -> `http://localhost:5170`
- HTTPS sigue disponible opcionalmente en `https://localhost:7203`.
- En Development no se fuerza redirección HTTP -> HTTPS.
- CORS de Development permite localhost/127.0.0.1:4200.
- Se agregó `GET /api/health` con validación de conectividad y tablas esperadas.
- La barra superior consulta realmente `/api/health` cada 10 segundos.
- Se agregó una base limpia separada `FmeaManagerDbDay22`, dejando V2 intacta.
- La migración consolidada fue normalizada como `InitialDay22`.
- En Development, `Database:AutoMigrate=true` aplica la migración al arrancar.
- Project Detail tolera un error de Traceability sin bloquear el resto de la vista.
- Mensajes HTTP muestran mejor errores de proxy y backend.
- En Development los HTTP 500 incluyen el mensaje real del servidor y traceId;
  producción continúa ocultando el detalle interno.
- Budgets CSS ajustados al tamaño real del proyecto.
- Se limpiaron artefactos `bin/obj/.vs/node_modules/dist` de los paquetes finales.
- Se agregaron scripts PowerShell de arranque y comprobación.

## Validaciones estáticas realizadas

- 53 rutas HTTP encontradas en el backend.
- 51 llamadas HTTP encontradas en los servicios Angular.
- Todas las llamadas del frontend tienen una ruta compatible en el backend.
- Todos los imports relativos TypeScript resuelven a archivos existentes.
- Los handlers usados por templates existen en sus componentes.
- JSON de configuración validado.
- La migración `InitialDay22` contiene las 20 tablas esperadas y seed para:
  - STANDARD
  - FORD
  - local-development (Read + Edit + Approve)

## Validación pendiente en tu Windows

Este entorno no tiene .NET 10 SDK y no tiene acceso de red a npm, por lo que
el build final debe ejecutarse en tu equipo:

Backend:
`dotnet restore`
`dotnet build`
`dotnet test`
`dotnet run --project .\FMEA-Api\FMEA-Api.csproj`

Frontend (Node 16.20.2 recomendado para Angular 14):
`npm install`
`npm start`

Después verifica:
`http://localhost:5170/api/health`
`http://localhost:4200/api/health`
