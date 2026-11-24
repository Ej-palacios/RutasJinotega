using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia.Configuraciones;

public sealed class LugarConfiguracion : IEntityTypeConfiguration<Lugar>
{
    public void Configure(EntityTypeBuilder<Lugar> builder)
    {
        builder.ToTable("Lugar");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(l => l.Descripcion).IsRequired().HasMaxLength(2000);
        builder.Property(l => l.Historia).HasMaxLength(4000);
        builder.Property(l => l.Direccion).HasMaxLength(250);
        builder.Property(l => l.Horario).HasMaxLength(250);
        builder.Property(l => l.Dificultad).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Estado).HasConversion<string>().HasMaxLength(20);

        // Coordenada es un value object: se guarda como dos columnas dentro de la tabla Lugar.
        builder.OwnsOne(l => l.Ubicacion, ubicacion =>
        {
            ubicacion.Property(c => c.Latitud).HasColumnName("Latitud");
            ubicacion.Property(c => c.Longitud).HasColumnName("Longitud");
        });
        builder.Navigation(l => l.Ubicacion).IsRequired();

        builder.HasOne(l => l.Municipio)
            .WithMany()
            .HasForeignKey(l => l.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Categorias)
            .WithMany()
            .UsingEntity(union => union.ToTable("LugarCategoria"));

        builder.HasMany(l => l.Fotos)
            .WithOne()
            .HasForeignKey(f => f.LugarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(l => l.Categorias).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(l => l.Fotos).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(l => l.Estado);
    }
}
