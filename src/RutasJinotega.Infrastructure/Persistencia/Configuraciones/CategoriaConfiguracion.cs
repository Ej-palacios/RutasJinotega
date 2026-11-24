using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia.Configuraciones;

public sealed class CategoriaConfiguracion : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categoria");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Icono).IsRequired().HasMaxLength(16);
        builder.Property(c => c.Color).IsRequired().HasMaxLength(16);
        builder.HasIndex(c => c.Nombre).IsUnique();
    }
}
