using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comidas_backend.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintForVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votos_ComentarioId",
                table: "Votos");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ComentarioId_UserId",
                table: "Votos",
                columns: new[] { "ComentarioId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votos_ComentarioId_UserId",
                table: "Votos");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ComentarioId",
                table: "Votos",
                column: "ComentarioId");
        }
    }
}
