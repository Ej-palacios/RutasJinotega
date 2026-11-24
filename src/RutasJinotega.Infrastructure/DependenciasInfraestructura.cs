using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Infrastructure.Persistencia;

namespace RutasJinotega.Infrastructure;

public static class DependenciasInfraestructura
{
    /// <summary>Registra EF Core (SQL Server) y las implementaciones de los puertos de Application.</summary>
    public static IServiceCollection AgregarInfraestructura(
        this IServiceCollection servicios,
        string cadenaConexion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cadenaConexion);

        servicios.AddDbContext<RutasJinotegaDbContext>(opciones => opciones.UseSqlServer(cadenaConexion));

        servicios.AddScoped<ILectorLugares, LectorLugares>();
        servicios.AddScoped<ILectorCatalogo, LectorCatalogo>();
        servicios.AddScoped<ILectorRutas, LectorRutas>();

        return servicios;
    }
}
