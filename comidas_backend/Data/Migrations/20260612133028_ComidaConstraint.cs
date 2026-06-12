using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comidas_backend.Migrations
{
    /// <inheritdoc />
    public partial class ComidaConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_ComidaId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ComidaId_UserId",
                table: "Calificaciones",
                columns: new[] { "ComidaId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_ComidaId_UserId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ComidaId",
                table: "Calificaciones",
                column: "ComidaId");
        }
    }
}
