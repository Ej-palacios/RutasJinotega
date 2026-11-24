using Xunit;
using FluentAssertions;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;
using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Domain.Tests;

public class RutaTests
{
    private static readonly Municipio SanRafael = Municipio.Crear("San Rafael del Norte");

    private static Ruta Crear() =>
        Ruta.Crear(SanRafael, "san-rafael", "Ruta turística San Rafael", "Descripción");

    private static Lugar CrearLugar(string nombre = "Tepeyac") =>
        Lugar.Crear(
            SanRafael,
            nombre,
            "Descripción",
            new Coordenada(13.2083, -86.1000),
            DificultadAcceso.Facil,
            false);

    [Fact]
    public void Crear_recorta_la_clave_y_el_nombre()
    {
        var ruta = Ruta.Crear(SanRafael, "  san-rafael  ", "  Ruta  ", "  Descripción  ");

        ruta.Clave.Should().Be("san-rafael");
        ruta.Nombre.Should().Be("Ruta");
        ruta.Descripcion.Should().Be("Descripción");
        ruta.EsCurada.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_sin_clave_lanza_excepcion(string clave)
    {
        var accion = () => Ruta.Crear(SanRafael, clave, "Ruta", "Descripción");

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Agregar_parada_con_orden_duplicado_lanza_excepcion()
    {
        var ruta = Crear();
        ruta.AgregarParada(CrearLugar(), 1);

        var accion = () => ruta.AgregarParada(CrearLugar("Otra"), 1);

        accion.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Agregar_parada_con_orden_invalido_lanza_excepcion(int orden)
    {
        var accion = () => Crear().AgregarParada(CrearLugar(), orden);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Agregar_parada_con_tiempo_negativo_lanza_excepcion()
    {
        var accion = () => Crear().AgregarParada(CrearLugar(), 1, -5);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Las_paradas_se_agregan_en_el_orden_indicado()
    {
        var ruta = Crear();
        var tepeyac = CrearLugar("Tepeyac");
        var pulperia = CrearLugar("Pulpería");

        ruta.AgregarParada(tepeyac, 2);
        ruta.AgregarParada(pulperia, 1);

        ruta.Paradas.Should().HaveCount(2);
        ruta.Paradas.Select(p => p.Orden).Should().Equal(2, 1);
        ruta.Paradas.Select(p => p.Lugar.Nombre).Should().Equal("Tepeyac", "Pulpería");
    }
}
