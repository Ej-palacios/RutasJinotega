using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RutasJinotega.Infrastructure.Persistencia;
using Xunit;

namespace RutasJinotega.Integration.Tests;

/// <summary>
/// Base de datos real y aislada en SQL Express. Cada ejecución crea una base con nombre único,
/// aplica las migraciones y la elimina al terminar. El servidor se puede cambiar con
/// RUTASJINOTEGA_TEST_SERVER (por ejemplo, otra instancia o un contenedor).
/// </summary>
public sealed class BaseDatosSqlExpress : IAsyncLifetime
{
    private const string ServidorPredeterminado =
        @"Server=localhost\SQLEXPRESS;Integrated Security=True;TrustServerCertificate=True;";

    private readonly string _nombre;

    public BaseDatosSqlExpress()
    {
        _nombre = $"RutasJinotega_Test_{Guid.NewGuid():N}";
        var servidor = Environment.GetEnvironmentVariable("RUTASJINOTEGA_TEST_SERVER")
            ?? ServidorPredeterminado;

        Servidor = servidor.TrimEnd(';');
        CadenaConexion = $"{Servidor};Database={_nombre};";
    }

    public string Servidor { get; }

    public string CadenaConexion { get; }

    public RutasJinotegaDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<RutasJinotegaDbContext>()
            .UseSqlServer(CadenaConexion)
            .Options;

        return new RutasJinotegaDbContext(opciones);
    }

    public async Task InitializeAsync()
    {
        await using (var conexion = new SqlConnection(Servidor))
        {
            await conexion.OpenAsync();
            await using var comando = conexion.CreateCommand();
            comando.CommandText = $"CREATE DATABASE [{_nombre}];";
            await comando.ExecuteNonQueryAsync();
        }

        await using var db = CrearContexto();
        await db.Database.MigrateAsync();
        await SembradorDatos.SembrarAsync(db);
    }

    public async Task DisposeAsync()
    {
        SqlConnection.ClearAllPools();

        await using var conexion = new SqlConnection(Servidor);
        await conexion.OpenAsync();

        await using var comando = conexion.CreateCommand();
        comando.CommandText = $"""
            IF DB_ID(N'{_nombre}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_nombre}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_nombre}];
            END;
            """;

        await comando.ExecuteNonQueryAsync();
    }
}
