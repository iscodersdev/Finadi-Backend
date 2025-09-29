using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalidadId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_LocalidadId",
                table: "Clientes",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ProvinciaId",
                table: "Clientes",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Localidad_LocalidadId",
                table: "Clientes",
                column: "LocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Provincia_ProvinciaId",
                table: "Clientes",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Localidad_LocalidadId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Provincia_ProvinciaId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_LocalidadId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_ProvinciaId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "LocalidadId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Clientes");
        }
    }
}
