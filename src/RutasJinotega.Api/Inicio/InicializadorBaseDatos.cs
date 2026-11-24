using Microsoft.EntityFrameworkCore;
using RutasJinotega.Infrastructure.Persistencia;

namespace RutasJinotega.Api.Inicio;

public static class InicializadorBaseDatos
{
    /// <summary>
    /// Aplica el esquema versionado y siembra el catálogo. Las bases creadas antes de existir
    /// migraciones reciben una línea base para que el cambio de esquema no las rompa.
    /// </summary>
    public static async Task InicializarBaseDeDatosAsync(this WebApplication app)
    {
        using var alcance = app.Services.CreateScope();
        var db = alcance.ServiceProvider.GetRequiredService<RutasJinotegaDbContext>();

        var migraciones = db.Database.GetMigrations().ToList();

        if (migraciones.Count == 0)
        {
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            await AsegurarLineaBaseAsync(db, migraciones[0]);
            await db.Database.MigrateAsync();
        }

        await SembradorDatos.SembrarAsync(db);
    }

    /// <summary>
    /// Si la base ya tiene tablas pero no historial de migraciones (creada con EnsureCreated),
    /// registra la primera migración como aplicada y agrega solo las columnas nuevas conocidas.
    /// </summary>
    private static async Task AsegurarLineaBaseAsync(
        RutasJinotegaDbContext db,
        string primeraMigracion)
    {
        if (!await db.Database.CanConnectAsync())
        {
            return;
        }

        await db.Database.OpenConnectionAsync();

        try
        {
            if (await EscalarAsync(db, "SELECT CASE WHEN OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL THEN 0 ELSE 1 END") == 1)
            {
                return;
            }

            if (await EscalarAsync(db, "SELECT CASE WHEN OBJECT_ID(N'[Lugar]') IS NULL THEN 0 ELSE 1 END") == 0)
            {
                return;
            }

            if (await EscalarAsync(db, "SELECT CASE WHEN COL_LENGTH(N'Lugar', N'Historia') IS NULL THEN 0 ELSE 1 END") == 0)
            {
                await db.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE [Lugar] ADD [Historia] nvarchar(4000) NULL;");
            }

            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END;
                """);

            var version = typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "10.0.0";

            await db.Database.ExecuteSqlInterpolatedAsync($"""
                IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {primeraMigracion})
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ({primeraMigracion}, {version});
                """);
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    private static async Task<int> EscalarAsync(RutasJinotegaDbContext db, string sql)
    {
        await using var comando = db.Database.GetDbConnection().CreateCommand();
        comando.CommandText = sql;
        return Convert.ToInt32(await comando.ExecuteScalarAsync());
    }
}
