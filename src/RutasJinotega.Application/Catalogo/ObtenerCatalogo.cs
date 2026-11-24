using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Application.Catalogo;

public sealed record ObtenerCatalogoConsulta;

public sealed class ObtenerCatalogoHandler(ILectorCatalogo lector)
    : IConsultaHandler<ObtenerCatalogoConsulta, CatalogoDto>
{
    public async Task<Resultado<CatalogoDto>> EjecutarAsync(
        ObtenerCatalogoConsulta consulta,
        CancellationToken ct = default)
    {
        var municipios = await lector.ListarMunicipiosAsync(ct);
        var categorias = await lector.ListarCategoriasAsync(ct);

        var catalogo = new CatalogoDto(
            municipios.Select(LugarMapeador.AMunicipio).ToList(),
            categorias.Select(LugarMapeador.ACategoria).ToList());

        return Resultado<CatalogoDto>.Exito(catalogo);
    }
}
