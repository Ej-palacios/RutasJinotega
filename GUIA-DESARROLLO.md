# Guía de instalación y continuación

Esta guía deja el proyecto funcionando desde cero en Windows y explica cómo seguir desarrollándolo sin romper lo que ya existe. Para el contexto de producto y arquitectura, lee `README.md`, `PRODUCT.md` y `docs/ARQUITECTURA.md`.

## 1. Qué hay en el proyecto

| Proyecto | Rol |
|---|---|
| `src/RutasJinotega.Domain` | Entidades y reglas del catálogo. No depende de nada. |
| `src/RutasJinotega.Application` | Casos de uso, puertos (interfaces), DTOs y mapeadores. |
| `src/RutasJinotega.Infrastructure` | EF Core + SQL Server, lectores, migraciones y datos semilla. |
| `src/RutasJinotega.Api` | API mínima; raíz de composición del backend. |
| `src/RutasJinotega.Shared.UI` | Componentes y páginas Razor + CSS glass compartidos. |
| `src/RutasJinotega.Web` | Blazor WebAssembly. |
| `src/RutasJinotega.Maui` | App Android con Blazor Hybrid. |
| `tests/RutasJinotega.Domain.Tests` | Pruebas de dominio. |
| `tests/RutasJinotega.Application.Tests` | Pruebas de casos de uso con dobles. |
| `tests/RutasJinotega.Integration.Tests` | Pruebas contra SQL Express real. |

Regla de dependencias: `Domain ← Application ← Infrastructure ← Api/Web/Maui`. Nada de capas internas debe nombrar capas externas.

## 2. Requisitos

Obligatorio:

- **Windows 10/11** (probado en Windows con SDK `10.0.403`).
- **.NET SDK 10** — https://dotnet.microsoft.com/download
- **SQL Server Express** con la instancia `SQLEXPRESS`, o Docker para levantar SQL Server en contenedor.
- **Git** (opcional si solo descomprimes el ZIP).

Opcional, solo para la app móvil:

- Workload de .NET para Android: `dotnet workload install android`.
- Android SDK + emulador o un teléfono con depuración USB.

Verifica las herramientas:

```powershell
dotnet --version
git --version
Get-Service 'MSSQL$SQLEXPRESS'
```

## 3. Puesta en marcha rápida

### Paso 1 — Descomprimir o clonar

Si recibiste un ZIP, descomprímelo en una ruta sin espacios problemáticos, por ejemplo `C:\Dev\RutasJinotega`.

Si vienes del repositorio:

```powershell
git clone https://github.com/Ej-palacios/RutasJinotega.git
cd RutasJinotega
```

### Paso 2 — Restaurar dependencias

```powershell
dotnet restore RutasJinotega.sln
```

### Paso 3 — Preparar SQL Server

**Opción A — SQL Server Express local (recomendada en Windows).**

1. Instala SQL Server Express y deja la instancia con nombre `SQLEXPRESS`.
2. Inicia el servicio:

```powershell
Get-Service 'MSSQL$SQLEXPRESS' | Start-Service
```

3. La cadena de conexión por defecto usa autenticación de Windows:

```json
"RutasJinotega": "Server=localhost\\SQLEXPRESS;Database=RutasJinotega;Trusted_Connection=True;TrustServerCertificate=True"
```

Está en `src/RutasJinotega.Api/appsettings.json`. Si tu instancia tiene otro nombre, cámbiala ahí.

**Opción B — SQL Server en Docker.**

```powershell
docker compose up -d
$env:ConnectionStrings__RutasJinotega = "Server=localhost,1433;Database=RutasJinotega;User Id=sa;Password=Dev_Passw0rd!;TrustServerCertificate=True"
```

La variable de entorno solo vive en esa terminal; abre la API desde la misma.

### Paso 4 — Arrancar la API

```powershell
dotnet run --project src/RutasJinotega.Api
```

- Escucha en `http://localhost:5100`.
- Al arrancar aplica las migraciones pendientes y siembra/refresca el catálogo. No necesitas crear la base a mano.
- Comprueba: `http://localhost:5100/api/rutas/san-rafael`.

### Paso 5 — Arrancar la web

En otra terminal:

```powershell
dotnet run --project src/RutasJinotega.Web
```

- Abre `http://localhost:5000`.
- La URL de la API se configura en `src/RutasJinotega.Web/wwwroot/appsettings.json` (`UrlApi`).

### Paso 6 — Verificación mínima

| Comprobación | Resultado esperado |
|---|---|
| `GET /api/rutas/san-rafael` | 5 paradas, ~1.2 km, termina en `El Tepeyac` |
| `/` | Catálogo con mapa de calles y 12 marcadores |
| `/rutas` | Mapa, 5 paradas y hoja de detalle |
| `/lugar/12` | El Tepeyac con sección `Historia` |
| Consola del navegador | Sin errores |

## 4. Configuración

| Qué | Dónde | Nota |
|---|---|---|
| Cadena de conexión | `src/RutasJinotega.Api/appsettings.json` | También se puede pisar con `ConnectionStrings__RutasJinotega`. |
| URL de la API (web) | `src/RutasJinotega.Web/wwwroot/appsettings.json` | Debe terminar en `/`. |
| URL de la API (MAUI) | `src/RutasJinotega.Maui/ConfiguracionApp.cs` | Emulador Android: `http://10.0.2.2:5100/`. Teléfono físico: IP LAN de tu PC. |
| Puertos | `src/RutasJinotega.Api/Properties/launchSettings.json` y perfil de Web | API `5100`, web `5000`. |
| CORS | `src/RutasJinotega.Api/Program.cs` | Abierto solo para desarrollo. Restringir antes de publicar. |

## 5. Base de datos

### Cómo se inicializa

`InicializadorBaseDatos` hace tres cosas al arrancar en Development:

1. Aplica las migraciones de `src/RutasJinotega.Infrastructure/Persistencia/Migraciones`.
2. Si la base viene de una versión anterior sin historial de migraciones, registra una línea base y agrega las columnas nuevas conocidas (por ejemplo `Lugar.Historia`) sin borrar datos.
3. Ejecuta `SembradorDatos`, que hace upsert por nombre + municipio y refresca contenido editorial y coordenadas.

Por eso puedes actualizar el proyecto sobre una base existente sin recrearla.

### Agregar un cambio de esquema

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add NombreDelCambio `
  --project src/RutasJinotega.Infrastructure `
  --startup-project src/RutasJinotega.Api `
  --output-dir Persistencia/Migraciones
```

Luego arranca la API: la migración se aplica sola en Development. Revisa el archivo generado antes de confirmarlo; si el cambio es destructivo, agrega el paso de datos correspondiente.

### Recrear la base desde cero

Solo si no necesitas los datos actuales:

```sql
ALTER DATABASE [RutasJinotega] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [RutasJinotega];
```

La API la vuelve a crear y sembrar al arrancar.

## 6. Pruebas

Todas:

```powershell
dotnet test RutasJinotega.sln
```

Solo una capa:

```powershell
dotnet test tests/RutasJinotega.Domain.Tests
dotnet test tests/RutasJinotega.Application.Tests
dotnet test tests/RutasJinotega.Integration.Tests
```

Las pruebas de integración necesitan SQL Express. Crean una base única por ejecución (`RutasJinotega_Test_<guid>`), aplican migraciones, siembran, prueban y la eliminan. Para usar otro servidor:

```powershell
$env:RUTASJINOTEGA_TEST_SERVER = "Server=localhost,1433;User Id=sa;Password=Dev_Passw0rd!;TrustServerCertificate=True;"
dotnet test tests/RutasJinotega.Integration.Tests
```

Estado verificado en la última entrega: **54 pruebas correctas** (28 dominio + 20 aplicación + 6 integración).

Si `dotnet test` de la solución falla por el proyecto MAUI, compila y prueba por separado; MAUI requiere el workload de Android instalado.

## 7. App móvil (opcional)

```powershell
dotnet workload install android
dotnet build src/RutasJinotega.Maui/RutasJinotega.Maui.csproj -f net10.0-android
```

- Emulador Android: la API se alcanza en `http://10.0.2.2:5100/`.
- Teléfono físico: cambia `ConfiguracionApp.UrlApi` por `http://<IP-LAN-de-tu-PC>:5100/` y permite el puerto en el firewall.
- La pantalla de rutas aún no se ha probado en dispositivo; es una de las tareas pendientes.

## 8. Cómo extender el proyecto

### Agregar o corregir un lugar

1. Edita `src/RutasJinotega.Infrastructure/Persistencia/Datos/DatosSemilla.cs`.
2. Usa nombre + municipio como clave estable; el sembrador actualiza descripción, historia, horario, dirección y coordenadas.
3. Arranca la API para refrescar la base.
4. Si el lugar aparece en una ruta, revisa `LectorRutas` (la ruta busca por nombre).

No inventes coordenadas ni historia. Verifica en OpenStreetMap/Wikipedia o en campo y anota la fuente en el contenido.

### Agregar o cambiar paradas de la ruta

La definición vive en `src/RutasJinotega.Infrastructure/Persistencia/LectorRutas.cs`. Cada parada tiene nombre de lugar, orden y nota. Si agregas una parada, actualiza también las pruebas de integración que esperan 5 paradas y el final en El Tepeyac.

### Agregar un caso de uso o endpoint

1. Crea el handler en `src/RutasJinotega.Application` implementando `IConsultaHandler<TConsulta, TRespuesta>`.
2. Registra el handler en `DependenciasAplicacion`.
3. Si necesita datos, agrega el puerto en `Application/Abstracciones` e impleméntalo en `Infrastructure`.
4. Expón el endpoint en `src/RutasJinotega.Api/Endpoints` y mapéalo en `Program.cs`.
5. Agrega pruebas de aplicación y, si toca SQL, de integración.

### Cambiar la UI

- Páginas: `src/RutasJinotega.Shared.UI/Paginas`.
- Componentes: `src/RutasJinotega.Shared.UI/Componentes`.
- Estilos: `src/RutasJinotega.Shared.UI/wwwroot/css/glass.css`.
- El mapa de calles está en `Componentes/MapaCalles.razor` + `wwwroot/js/mapa-calles.js`; `MapaRuta` y `MapaLugares` son adaptadores.
- Mantén el estilo glass/iOS y prueba móvil + escritorio (breakpoint en 760 px).

## 9. Problemas comunes

| Síntoma | Causa probable | Solución |
|---|---|---|
| `Falta la cadena de conexión 'RutasJinotega'` | `appsettings.json` incompleto | Revisa la sección `ConnectionStrings`. |
| Error de conexión a SQL | Servicio detenido o instancia con otro nombre | `Get-Service 'MSSQL$SQLEXPRESS' \| Start-Service` o ajusta `Server=`. |
| `The process cannot access the file ... because it is being used by another process` | API/Web siguen corriendo durante el build | Cierra los procesos en los puertos 5100/5000 y recompila. |
| La web carga pero no hay datos | API apagada o `UrlApi` incorrecta | Arranca la API y revisa `wwwroot/appsettings.json`. |
| El mapa sale esquemático | Sin internet o CDN bloqueado | Leaflet/OSM se cargan desde unpkg; habilita red o usa el respaldo. |
| `dotnet ef` no se reconoce | Herramienta no instalada | `dotnet tool install --global dotnet-ef` y abre una terminal nueva. |
| Falla el build de MAUI | Falta workload/SDK de Android | `dotnet workload install android`; compila API/Web por separado. |
| Las pruebas de integración fallan | Sin SQL Express o sin permisos para crear bases | Inicia SQL Express; la cuenta debe poder `CREATE DATABASE`. |

## 10. Limitaciones y siguientes pasos

Limitaciones actuales:

- Nombres y coordenadas de las mini pulperías siguen siendo datos de ejemplo.
- El mapa depende de internet (Leaflet + teselas de OpenStreetMap).
- La ruta está definida en código, no en base de datos.
- No hay autenticación, reseñas, favoritos ni modo offline.
- MAUI no se ha probado en emulador ni dispositivo.
- No hay CI.

Siguiente paso recomendado: Fase 2 del roadmap (Identity + JWT, reseñas con fotos y favoritos) o cerrar los datos reales de la ruta. El detalle está en `docs/ARQUITECTURA.md`.

## 11. Publicar en GitHub

El repositorio de referencia es:

```text
https://github.com/Ej-palacios/RutasJinotega
```

Para subir cambios:

```powershell
git status
git add .
git commit -m "Descripción del cambio"
git push
```

Antes de publicar, revisa que no haya cadenas de conexión con contraseñas, tokens ni archivos de `bin/`, `obj/` o `.vs/`. El `.gitignore` ya excluye lo habitual.
