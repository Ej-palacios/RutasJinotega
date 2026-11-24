using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia.Configuraciones;

public sealed class FotoLugarConfiguracion : IEntityTypeConfiguration<FotoLugar>
{
    public void Configure(EntityTypeBuilder<FotoLugar> builder)
    {
        builder.ToTable("FotoLugar");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Url).IsRequired().HasMaxLength(500);
    }
}
