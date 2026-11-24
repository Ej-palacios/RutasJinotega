using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia.Configuraciones;

public sealed class MunicipioConfiguracion : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.ToTable("Municipio");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Descripcion).HasMaxLength(500);
        builder.HasIndex(m => m.Nombre).IsUnique();
    }
}
