using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RutasJinotega.Application;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;
using RutasJinotega.Domain.Comun;
using RutasJinotega.Infrastructure;
using RutasJinotega.Infrastructure.Persistencia;
using Xunit;

namespace RutasJinotega.Integration.Tests;

public sealed class PersistenciaSqlExpressTests(BaseDatosSqlExpress bd)
    : IClassFixture<BaseDatosSqlExpress>
{
    [Fact]
    public async Task Las_migraciones_crean_el_esquema_versionado()
    {
        await using var db = bd.CrearContexto();

        var aplicadas = await db.Database.GetAppliedMigrationsAsync();
        var columnas = await db.Database
            .SqlQueryRaw<string>(
                "SELECT COLUMN_NAME AS Value FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Lugar'")
            .ToListAsync();

        aplicadas.Should().NotBeEmpty();
        columnas.Should()
            .Contain("Nombre")
            .And.Contain("Descripcion")
            .And.Contain("Historia")
            .And.Contain("Latitud")
            .And.Contain("Longitud");
    }

    [Fact]
    public async Task El_sembrador_es_idempotente()
    {
        await using var db = bd.CrearContexto();

        var antes = await db.Lugares.CountAsync();
        await SembradorDatos.SembrarAsync(db);
        var despues = await db.Lugares.CountAsync();

        antes.Should().BeGreaterThan(0);
        despues.Should().Be(antes);
    }

    [Fact]
    public async Task La_ruta_san_rafael_se_resuelve_desde_sql_express()
    {
        using var proveedor = CrearProveedor();
        using var alcance = proveedor.CreateScope();
        var handler = alcance.ServiceProvider
            .GetRequiredService<IConsultaHandler<ObtenerRutaConsulta, RutaDetalleDto>>();

        var resultado = await handler.EjecutarAsync(new ObtenerRutaConsulta("san-rafael"));

        resultado.EsExito.Should().BeTrue();
        var ruta = resultado.Valor!;
        ruta.Paradas.Should().HaveCount(5);
        ruta.Paradas.Select(p => p.Orden).Should().BeInAscendingOrder();
        ruta.Paradas[0].Lugar.Nombre.Should().Contain("Mini pulpería");
        ruta.Paradas[^1].Lugar.Nombre.Should().Be("El Tepeyac");
        ruta.DistanciaTotalKm.Should().BeGreaterThan(0.5);
        ruta.DuracionTotalMin.Should().Be(65);
    }

    [Fact]
    public async Task Los_referentes_culturales_tienen_historia_y_coordenadas_verificadas()
    {
        await using var db = bd.CrearContexto();

        var tepeyac = await db.Lugares.FirstAsync(l => l.Nombre == "El Tepeyac");
        var catedral = await db.Lugares.FirstAsync(l => l.Nombre == "Catedral de San Juan Bautista");
        var iglesia = await db.Lugares.FirstAsync(l => l.Nombre == "Iglesia de San Rafael del Norte");

        tepeyac.Historia.Should().NotBeNullOrWhiteSpace().And.Contain("Odorico");
        tepeyac.Ubicacion.Latitud.Should().BeApproximately(13.21895, 0.001);
        tepeyac.Ubicacion.Longitud.Should().BeApproximately(-86.10808, 0.001);

        catedral.Historia.Should().NotBeNullOrWhiteSpace().And.Contain("1958");
        catedral.Ubicacion.Latitud.Should().BeApproximately(13.09191, 0.001);

        iglesia.Historia.Should().NotBeNullOrWhiteSpace().And.Contain("Sandino");
        iglesia.Ubicacion.Latitud.Should().BeApproximately(13.21351, 0.001);
    }

    [Fact]
    public async Task El_detalle_expone_la_historia_al_cliente()
    {
        await using var db = bd.CrearContexto();
        var tepeyac = await db.Lugares.AsNoTracking().FirstAsync(l => l.Nombre == "El Tepeyac");

        using var proveedor = CrearProveedor();
        using var alcance = proveedor.CreateScope();
        var handler = alcance.ServiceProvider
            .GetRequiredService<IConsultaHandler<ObtenerDetalleLugarConsulta, LugarDetalleDto>>();

        var resultado = await handler.EjecutarAsync(new ObtenerDetalleLugarConsulta(tepeyac.Id));

        resultado.EsExito.Should().BeTrue();
        resultado.Valor!.Historia.Should().NotBeNullOrWhiteSpace().And.Contain("Santuario");
    }

    [Fact]
    public async Task El_sembrador_refresca_el_contenido_de_una_base_existente()
    {
        await using (var db = bd.CrearContexto())
        {
            var tepeyac = await db.Lugares.FirstAsync(l => l.Nombre == "El Tepeyac");
            tepeyac.ActualizarContenido("Contenido viejo", null, null, null, new Coordenada(0, 0));
            await db.SaveChangesAsync();
        }

        await using (var db = bd.CrearContexto())
        {
            await SembradorDatos.SembrarAsync(db);
        }

        await using (var db = bd.CrearContexto())
        {
            var tepeyac = await db.Lugares.AsNoTracking().FirstAsync(l => l.Nombre == "El Tepeyac");
            tepeyac.Descripcion.Should().NotBe("Contenido viejo");
            tepeyac.Historia.Should().NotBeNullOrWhiteSpace();
            tepeyac.Ubicacion.Latitud.Should().BeApproximately(13.21895, 0.001);
        }
    }

    private ServiceProvider CrearProveedor()
    {
        var servicios = new ServiceCollection();
        servicios.AgregarAplicacion();
        servicios.AgregarInfraestructura(bd.CadenaConexion);
        return servicios.BuildServiceProvider();
    }
}
