# Rutas Jinotega

App tipo Waze enfocada en **rutas turísticas de Jinotega y sus municipios**: lugares conocidos y poco conocidos, ubicación por GPS y (próximamente) reseñas reales de visitantes.

**Stack:** .NET 10 · .NET MAUI Blazor Hybrid (móvil) · Blazor WebAssembly (web) · ASP.NET Core (API) · EF Core + SQL Server · estilo glassmorphism / iOS.

> **Estado: Fase 0 + Fase 1 (MVP de catálogo, solo lectura).** Lee `docs/ARQUITECTURA.md` (sección 11) para ver qué se verificó y qué falta por probar.
>
> **¿Vas a instalar o continuar el proyecto?** Sigue `GUIA-DESARROLLO.md`: requisitos, SQL Server, arranque, migraciones, pruebas, app móvil y problemas comunes.

## Estructura

```
RutasJinotega.sln
├── src/
│   ├── RutasJinotega.Domain          Entidades y reglas (sin dependencias)
│   ├── RutasJinotega.Application     Casos de uso, puertos (interfaces) y DTOs
│   ├── RutasJinotega.Infrastructure  EF Core, repositorios, datos de ejemplo
│   ├── RutasJinotega.Api             API mínima (raíz de composición del backend)
│   ├── RutasJinotega.Shared.UI       Componentes y páginas Razor + CSS glass (compartido)
│   ├── RutasJinotega.Web             Blazor WebAssembly
│   └── RutasJinotega.Maui            App móvil (Android por ahora)
├── tests/
│   ├── RutasJinotega.Domain.Tests
│   ├── RutasJinotega.Application.Tests
│   └── RutasJinotega.Integration.Tests   SQL Express real: migraciones, siembra y ruta
└── docs/
    ├── ARQUITECTURA.md               Arquitectura, modelo de datos, roadmap y estado
    └── DISENO-UI.html                Guía visual del estilo glass (ábrela en el navegador)
```

**Regla de dependencias:** `Domain ← Application ← Infrastructure`. La UI solo conoce `Application` y habla con la API mediante `ILugaresClient`. Lo específico de cada plataforma (GPS) va detrás de `IServicioUbicacion`.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server: **SQL Express** (`localhost\SQLEXPRESS`), **LocalDB** (viene con Visual Studio en Windows) o Docker (`docker compose up -d`)
- Para MAUI: `dotnet workload install maui` y Android SDK / emulador

## Cómo ejecutarlo

```bash
# 1. Restaurar y compilar (sin MAUI)
dotnet build src/RutasJinotega.Api
dotnet build src/RutasJinotega.Web

# 2. API (crea la base de datos y siembra datos de ejemplo en Development)
dotnet run --project src/RutasJinotega.Api          # http://localhost:5100

# 3. Web (en otra terminal)
dotnet run --project src/RutasJinotega.Web
```

- **Cadena de conexión:** `src/RutasJinotega.Api/appsettings.json`. Con Docker, usa la que indica `docker-compose.yml`.
- **URL de la API para la web:** `src/RutasJinotega.Web/wwwroot/appsettings.json`.
- **MAUI:** `dotnet build src/RutasJinotega.Maui -t:Run -f net10.0-android`. El emulador accede a la API en `http://10.0.2.2:5100/`; en un teléfono físico cambia `ConfiguracionApp.UrlApi` por la IP de tu PC.

### Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/catalogo` | Municipios y categorías |
| GET | `/api/lugares?municipioId=&categoriaId=&soloPocoConocidos=&texto=` | Lista con filtros |
| GET | `/api/lugares/cercanos?latitud=&longitud=&radioKm=&maximo=&categoriaId=` | Lugares cercanos, ordenados por distancia |
| GET | `/api/lugares/{id}` | Detalle de un lugar |
| GET | `/api/rutas/{clave}` | Ruta curada con paradas ordenadas (ej. `san-rafael`) |

## Tests

```bash
dotnet test RutasJinotega.sln
```

| Proyecto | Qué cubre |
|---|---|
| `tests/RutasJinotega.Domain.Tests` | Entidades, value objects y reglas del catálogo |
| `tests/RutasJinotega.Application.Tests` | Casos de uso y mapeadores con dobles de prueba |
| `tests/RutasJinotega.Integration.Tests` | SQL Express real: migraciones, siembra idempotente, ruta `san-rafael` y contenido histórico |

Las pruebas de integración crean una base única por ejecución, aplican las migraciones, siembran y la eliminan al terminar. Usan `localhost\SQLEXPRESS` por defecto; para otro servidor:

```powershell
$env:RUTASJINOTEGA_TEST_SERVER = "Server=mi-servidor;Integrated Security=True;TrustServerCertificate=True;"
dotnet test tests/RutasJinotega.Integration.Tests
```

## Migraciones

El esquema está versionado en `src/RutasJinotega.Infrastructure/Persistencia/Migraciones`. La API aplica las migraciones pendientes y luego siembra el catálogo al arrancar; las bases creadas antes de existir migraciones reciben una línea base automática (incluida la columna `Historia`), así que no hace falta borrarlas.

Para agregar un cambio de esquema:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add NombreDelCambio -p src/RutasJinotega.Infrastructure -s src/RutasJinotega.Api -o Persistencia/Migraciones
```

## Importante

- **Los datos semilla son de ejemplo.** Las coordenadas son aproximadas y los lugares marcados "(Ejemplo)" son ficticios. Sustitúyelos por lugares reales verificados (`Infrastructure/Persistencia/Datos/DatosSemilla.cs`).
- **El mapa usa Leaflet + OpenStreetMap** (teselas reales) con respaldo esquemático si no hay red o no carga el script. Requiere internet en el navegador.
- **No uses la configuración de desarrollo en producción:** CORS abierto, tráfico HTTP sin cifrar en Android y cadena de conexión local.

## Siguiente paso

Fase 2 del roadmap: cuentas (Identity + JWT), reseñas con fotos y favoritos. El detalle completo está en `docs/ARQUITECTURA.md`.
