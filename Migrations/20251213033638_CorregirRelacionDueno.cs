using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reservasApp.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRelacionDueno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurantes_Usuarios_DueñoId",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_DueñoId",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "DueñoId",
                table: "Restaurantes");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_UsuarioId",
                table: "Restaurantes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurantes_Usuarios_UsuarioId",
                table: "Restaurantes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurantes_Usuarios_UsuarioId",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_UsuarioId",
                table: "Restaurantes");

            migrationBuilder.AddColumn<int>(
                name: "DueñoId",
                table: "Restaurantes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_DueñoId",
                table: "Restaurantes",
                column: "DueñoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurantes_Usuarios_DueñoId",
                table: "Restaurantes",
                column: "DueñoId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
