using Xunit;
using FluentAssertions;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;
using RutasJinotega.Application.Tests.Dobles;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;
using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Application.Tests;

public class ObtenerRutaTests
{
    private static readonly Municipio SanRafael = Municipio.Crear("San Rafael del Norte");

    private static Lugar CrearLugar(string nombre, double latitud, double longitud) =>
        Lugar.Crear(
            SanRafael,
            nombre,
            "Descripción",
            new Coordenada(latitud, longitud),
            DificultadAcceso.Facil,
            false);

    private static Ruta CrearRuta(params Lugar[] paradas)
    {
        var ruta = Ruta.Crear(
            SanRafael,
            "san-rafael",
            "Ruta turística San Rafael",
            "De las mini pulperías al Tepeyac.");

        for (var i = 0; i < paradas.Length; i++)
        {
            ruta.AgregarParada(paradas[i], i + 1, 10);
        }

        return ruta;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Rechaza_claves_vacias(string clave)
    {
        var handler = new ObtenerRutaHandler(new LectorRutasEnMemoria([]));

        var resultado = await handler.EjecutarAsync(new ObtenerRutaConsulta(clave));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoValidacion);
    }

    [Fact]
    public async Task Devuelve_no_encontrado_si_la_ruta_no_existe()
    {
        var handler = new ObtenerRutaHandler(new LectorRutasEnMemoria([]));

        var resultado = await handler.EjecutarAsync(new ObtenerRutaConsulta("otra"));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoNoEncontrado);
    }

    [Fact]
    public async Task Devuelve_las_paradas_ordenadas_con_distancia_acumulada()
    {
        var primera = CrearLugar("Pulpería", 13.2080, -86.1005);
        var segunda = CrearLugar("Iglesia", 13.2083, -86.1000);
        var ruta = CrearRuta(primera, segunda);
        var handler = new ObtenerRutaHandler(new LectorRutasEnMemoria([ruta]));

        var resultado = await handler.EjecutarAsync(new ObtenerRutaConsulta(" SAN-RAFAEL "));

        resultado.EsExito.Should().BeTrue();
        var detalle = resultado.Valor!;
        detalle.Clave.Should().Be("san-rafael");
        detalle.Paradas.Should().HaveCount(2);
        detalle.Paradas.Select(p => p.Orden).Should().Equal(1, 2);
        detalle.Paradas[0].DistanciaAcumuladaKm.Should().Be(0);
        detalle.Paradas[1].DistanciaAcumuladaKm.Should().BeGreaterThan(0);
        detalle.DistanciaTotalKm.Should().Be(detalle.Paradas[1].DistanciaAcumuladaKm);
        detalle.DuracionTotalMin.Should().Be(20);
    }
}
