using Microsoft.EntityFrameworkCore;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia;

/// <summary>Unidad de trabajo de EF Core. La configuración de cada entidad vive en su propia clase.</summary>
public sealed class RutasJinotegaDbContext(DbContextOptions<RutasJinotegaDbContext> opciones)
    : DbContext(opciones)
{
    public DbSet<Lugar> Lugares => Set<Lugar>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<FotoLugar> FotosLugar => Set<FotoLugar>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RutasJinotegaDbContext).Assembly);
}
