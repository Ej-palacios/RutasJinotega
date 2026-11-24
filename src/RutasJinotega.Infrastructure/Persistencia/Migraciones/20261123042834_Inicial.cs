using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RutasJinotega.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lugar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MunicipioId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Historia = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Horario = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Dificultad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsPocoConocido = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PromedioCalificacion = table.Column<double>(type: "float", nullable: false),
                    TotalResenas = table.Column<int>(type: "int", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lugar_Municipio_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FotoLugar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LugarId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoLugar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotoLugar_Lugar_LugarId",
                        column: x => x.LugarId,
                        principalTable: "Lugar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LugarCategoria",
                columns: table => new
                {
                    CategoriasId = table.Column<int>(type: "int", nullable: false),
                    LugarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LugarCategoria", x => new { x.CategoriasId, x.LugarId });
                    table.ForeignKey(
                        name: "FK_LugarCategoria_Categoria_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LugarCategoria_Lugar_LugarId",
                        column: x => x.LugarId,
                        principalTable: "Lugar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Nombre",
                table: "Categoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FotoLugar_LugarId",
                table: "FotoLugar",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugar_Estado",
                table: "Lugar",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Lugar_MunicipioId",
                table: "Lugar",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_LugarCategoria_LugarId",
                table: "LugarCategoria",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipio_Nombre",
                table: "Municipio",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotoLugar");

            migrationBuilder.DropTable(
                name: "LugarCategoria");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Lugar");

            migrationBuilder.DropTable(
                name: "Municipio");
        }
    }
}
