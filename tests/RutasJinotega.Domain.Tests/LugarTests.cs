using Xunit;
using FluentAssertions;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Tests;

public class LugarTests
{
    private static readonly Municipio Jinotega = Municipio.Crear("Jinotega");

    private static Lugar Crear(string nombre = "Mirador") =>
        Lugar.Crear(Jinotega, nombre, "Descripción", new Coordenada(13.09, -86.0), DificultadAcceso.Facil, true);

    [Fact]
    public void Crear_sin_nombre_lanza_excepcion()
    {
        var accion = () => Crear("   ");

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Crear_recorta_el_nombre_y_inicia_activo()
    {
        var lugar = Crear("  Mirador  ");

        lugar.Nombre.Should().Be("Mirador");
        lugar.Estado.Should().Be(EstadoLugar.Activo);
        lugar.EsPocoConocido.Should().BeTrue();
    }

    [Fact]
    public void Agregar_la_misma_categoria_dos_veces_no_la_duplica()
    {
        var lugar = Crear();
        var categoria = Categoria.Crear("Naturaleza", "🌿", "#2fa56f");

        lugar.AgregarCategoria(categoria);
        lugar.AgregarCategoria(categoria);

        lugar.Categorias.Should().ContainSingle();
    }

    [Fact]
    public void Las_fotos_se_ordenan_en_el_orden_en_que_se_agregan()
    {
        var lugar = Crear();

        lugar.AgregarFoto("https://ejemplo.test/1.jpg");
        lugar.AgregarFoto("https://ejemplo.test/2.jpg");

        lugar.Fotos.Select(f => f.Orden).Should().Equal(0, 1);
    }

    [Theory]
    [InlineData(-0.1, 1)]
    [InlineData(5.1, 1)]
    [InlineData(4.0, -1)]
    public void Actualizar_calificacion_fuera_de_rango_lanza_excepcion(double promedio, int total)
    {
        var lugar = Crear();

        var accion = () => lugar.ActualizarCalificacion(promedio, total);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Actualizar_calificacion_valida_redondea_a_dos_decimales()
    {
        var lugar = Crear();

        lugar.ActualizarCalificacion(4.456, 12);

        lugar.PromedioCalificacion.Should().Be(4.46);
        lugar.TotalResenas.Should().Be(12);
    }

    [Fact]
    public void Ocultar_y_activar_cambian_el_estado()
    {
        var lugar = Crear();

        lugar.Ocultar();
        lugar.Estado.Should().Be(EstadoLugar.Oculto);

        lugar.Activar();
        lugar.Estado.Should().Be(EstadoLugar.Activo);
    }
}
