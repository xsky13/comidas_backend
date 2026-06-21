using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comidas_backend.Migrations
{
    /// <inheritdoc />
    public partial class ComidaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalificacionUsuario",
                table: "Comidas");

            migrationBuilder.DropColumn(
                name: "UsuarioCalifica",
                table: "Comidas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalificacionUsuario",
                table: "Comidas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UsuarioCalifica",
                table: "Comidas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
