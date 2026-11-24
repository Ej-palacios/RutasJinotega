using Xunit;
using FluentAssertions;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Tests.Dobles;

namespace RutasJinotega.Application.Tests;

public class ObtenerDetalleLugarTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Rechaza_identificadores_no_validos(int id)
    {
        var handler = new ObtenerDetalleLugarHandler(new LectorLugaresEnMemoria([]));

        var resultado = await handler.EjecutarAsync(new ObtenerDetalleLugarConsulta(id));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoValidacion);
    }

    [Fact]
    public async Task Devuelve_no_encontrado_si_el_lugar_no_existe()
    {
        var handler = new ObtenerDetalleLugarHandler(new LectorLugaresEnMemoria([]));

        var resultado = await handler.EjecutarAsync(new ObtenerDetalleLugarConsulta(99));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoNoEncontrado);
    }
}
