using Xunit;
using FluentAssertions;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Tests.Dobles;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Application.Tests;

public class ObtenerLugaresCercanosTests
{
    private static readonly Municipio Municipio = Municipio.Crear("Jinotega");

    private static Lugar Lugar(string nombre, double latitud, double longitud) =>
        Domain.Catalogo.Lugar.Crear(
            Municipio, nombre, "desc", new Coordenada(latitud, longitud), DificultadAcceso.Facil, false);

    private static ObtenerLugaresCercanosHandler CrearHandler(params Lugar[] lugares) =>
        new(new LectorLugaresEnMemoria(lugares));

    [Fact]
    public async Task Devuelve_los_lugares_ordenados_del_mas_cercano_al_mas_lejano()
    {
        var handler = CrearHandler(
            Lugar("Lejos", 13.25, -86.00),
            Lugar("Cerca", 13.10, -86.00),
            Lugar("Medio", 13.17, -86.00));

        var resultado = await handler.EjecutarAsync(new ObtenerLugaresCercanosConsulta(13.09, -86.00, 50));

        resultado.EsExito.Should().BeTrue();
        resultado.Valor.Select(x => x.Lugar.Nombre).Should().Equal("Cerca", "Medio", "Lejos");
        resultado.Valor.Select(x => x.DistanciaKm).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Excluye_los_lugares_fuera_del_radio_aunque_esten_dentro_del_cuadrante()
    {
        // Una esquina del cuadrante queda más lejos que el radio: el filtro exacto debe descartarla.
        var origen = new Coordenada(13.09, -86.00);
        var cuadrante = origen.Alrededor(25);
        var esquina = Lugar("Esquina", cuadrante.Norte - 0.0001, cuadrante.Este - 0.0001);
        var handler = CrearHandler(esquina);

        var resultado = await handler.EjecutarAsync(new ObtenerLugaresCercanosConsulta(13.09, -86.00, 25));

        resultado.EsExito.Should().BeTrue();
        resultado.Valor.Should().BeEmpty();
    }

    [Fact]
    public async Task Respeta_el_maximo_de_resultados()
    {
        var handler = CrearHandler(
            Lugar("A", 13.091, -86.00),
            Lugar("B", 13.092, -86.00),
            Lugar("C", 13.093, -86.00));

        var resultado = await handler.EjecutarAsync(new ObtenerLugaresCercanosConsulta(13.09, -86.00, 10, 2));

        resultado.Valor.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(95, 0, 10, 10)]
    [InlineData(0, 200, 10, 10)]
    [InlineData(13, -86, 0, 10)]
    [InlineData(13, -86, -5, 10)]
    [InlineData(13, -86, 201, 10)]
    [InlineData(13, -86, 10, 0)]
    [InlineData(13, -86, 10, 101)]
    public async Task Rechaza_parametros_invalidos_con_error_de_validacion(
        double latitud, double longitud, double radio, int maximo)
    {
        var handler = CrearHandler();

        var resultado = await handler.EjecutarAsync(
            new ObtenerLugaresCercanosConsulta(latitud, longitud, radio, maximo));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoValidacion);
    }

    [Fact]
    public async Task Rechaza_coordenadas_que_no_son_numeros()
    {
        var handler = CrearHandler();

        var resultado = await handler.EjecutarAsync(
            new ObtenerLugaresCercanosConsulta(double.NaN, double.NaN));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoValidacion);
    }
}
