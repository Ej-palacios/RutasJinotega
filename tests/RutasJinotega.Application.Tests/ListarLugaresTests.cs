using Xunit;
using FluentAssertions;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Tests.Dobles;

namespace RutasJinotega.Application.Tests;

public class ListarLugaresTests
{
    [Fact]
    public async Task Rechaza_un_texto_de_busqueda_demasiado_largo()
    {
        var handler = new ListarLugaresHandler(new LectorLugaresEnMemoria([]));
        var texto = new string('a', FiltroLugares.LongitudMaximaTexto + 1);

        var resultado = await handler.EjecutarAsync(new ListarLugaresConsulta(new FiltroLugares(Texto: texto)));

        resultado.EsExito.Should().BeFalse();
        resultado.Error!.Codigo.Should().Be(Error.CodigoValidacion);
    }

    [Fact]
    public async Task Sin_lugares_devuelve_una_lista_vacia_exitosa()
    {
        var handler = new ListarLugaresHandler(new LectorLugaresEnMemoria([]));

        var resultado = await handler.EjecutarAsync(new ListarLugaresConsulta(new FiltroLugares()));

        resultado.EsExito.Should().BeTrue();
        resultado.Valor.Should().BeEmpty();
    }
}
