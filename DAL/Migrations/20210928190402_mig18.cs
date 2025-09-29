using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Vendedores");

            migrationBuilder.DropColumn(
                name: "NroDocumento",
                table: "Vendedores");

            migrationBuilder.AddColumn<int>(
                name: "PersonaId",
                table: "Vendedores",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendedores_PersonaId",
                table: "Vendedores",
                column: "PersonaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendedores_Personas_PersonaId",
                table: "Vendedores",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendedores_Personas_PersonaId",
                table: "Vendedores");

            migrationBuilder.DropIndex(
                name: "IX_Vendedores_PersonaId",
                table: "Vendedores");

            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Vendedores");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Vendedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NroDocumento",
                table: "Vendedores",
                nullable: true);
        }
    }
}
