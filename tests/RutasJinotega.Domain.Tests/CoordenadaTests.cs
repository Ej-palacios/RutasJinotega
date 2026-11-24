using Xunit;
using FluentAssertions;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Tests;

public class CoordenadaTests
{
    [Theory]
    [InlineData(91, 0)]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    [InlineData(0, -181)]
    [InlineData(double.NaN, 0)]
    public void Crear_con_valores_fuera_de_rango_lanza_excepcion(double latitud, double longitud)
    {
        var accion = () => new Coordenada(latitud, longitud);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Distancia_entre_el_mismo_punto_es_cero()
    {
        var punto = new Coordenada(13.0910, -85.9996);

        punto.DistanciaKm(punto).Should().BeApproximately(0, 1e-9);
    }

    [Fact]
    public void Distancia_de_un_grado_de_latitud_es_aproximadamente_111_km()
    {
        var a = new Coordenada(13, -86);
        var b = new Coordenada(14, -86);

        a.DistanciaKm(b).Should().BeApproximately(111.2, 0.5);
    }

    [Fact]
    public void Distancia_es_simetrica()
    {
        var a = new Coordenada(13.09, -86.00);
        var b = new Coordenada(13.21, -86.10);

        a.DistanciaKm(b).Should().BeApproximately(b.DistanciaKm(a), 1e-9);
    }

    [Fact]
    public void Cuadrante_contiene_los_puntos_que_estan_dentro_del_radio()
    {
        var origen = new Coordenada(13.09, -86.00);
        var cuadrante = origen.Alrededor(25);

        // Punto a ~20 km hacia el este y ~20 km hacia el norte (dentro del radio en cada eje).
        var cercano = new Coordenada(13.09 + 20 / 111.0, -86.00 + 20 / 111.0);

        cuadrante.Contiene(cercano).Should().BeTrue();
    }

    [Fact]
    public void Cuadrante_excluye_puntos_muy_lejanos()
    {
        var origen = new Coordenada(13.09, -86.00);
        var cuadrante = origen.Alrededor(25);

        cuadrante.Contiene(new Coordenada(14.5, -86.00)).Should().BeFalse();
    }

    [Fact]
    public void Cuadrante_nunca_deja_fuera_un_punto_que_esta_a_menos_del_radio()
    {
        var origen = new Coordenada(13.09, -86.00);
        var cuadrante = origen.Alrededor(30);

        foreach (var rumbo in Enumerable.Range(0, 360).Where(r => r % 15 == 0))
        {
            var radianes = rumbo * Math.PI / 180;
            var distanciaKm = 29.0;
            var lat = origen.Latitud + distanciaKm * Math.Cos(radianes) / 111.19;
            var lon = origen.Longitud + distanciaKm * Math.Sin(radianes) / (111.19 * Math.Cos(origen.Latitud * Math.PI / 180));
            var punto = new Coordenada(lat, lon);

            origen.DistanciaKm(punto).Should().BeLessThan(30);
            cuadrante.Contiene(punto).Should().BeTrue($"el rumbo {rumbo}° está dentro del radio");
        }
    }
}
