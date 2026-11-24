using RutasJinotega.Api.Endpoints;
using RutasJinotega.Api.Inicio;
using RutasJinotega.Application;
using RutasJinotega.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("RutasJinotega")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'RutasJinotega' en appsettings.json.");

// Raíz de composición: aquí (y solo aquí) se conectan las capas.
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(cadenaConexion);

builder.Services.AddCors(opciones => opciones.AddDefaultPolicy(politica =>
    politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())); // Solo desarrollo; restringir en producción.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InicializarBaseDeDatosAsync();
}

app.UseCors();

app.MapGet("/", () => Results.Ok(new { servicio = "Rutas Jinotega API", estado = "ok" }));
app.MapLugares();
app.MapCatalogo();
app.MapRutas();

app.Run();

public partial class Program;
