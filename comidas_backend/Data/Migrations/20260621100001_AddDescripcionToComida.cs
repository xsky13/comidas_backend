using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comidas_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDescripcionToComida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Comidas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Comidas");
        }
    }
}
