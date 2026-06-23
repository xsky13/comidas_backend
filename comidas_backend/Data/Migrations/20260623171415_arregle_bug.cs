using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comidas_backend.Migrations
{
    /// <inheritdoc />
    public partial class arregle_bug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Users_UserId",
                table: "Calificaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Users_UserId",
                table: "Calificaciones",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Users_UserId",
                table: "Calificaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Users_UserId",
                table: "Calificaciones",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
