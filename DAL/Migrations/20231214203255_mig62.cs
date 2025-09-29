using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig62 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Provincia",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Localidad",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "Localidad",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_PaisId",
                table: "Provincia",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_ProvinciaId",
                table: "Localidad",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Localidad_Provincia_ProvinciaId",
                table: "Localidad",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincia_Paises_PaisId",
                table: "Provincia",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localidad_Provincia_ProvinciaId",
                table: "Localidad");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincia_Paises_PaisId",
                table: "Provincia");

            migrationBuilder.DropIndex(
                name: "IX_Provincia_PaisId",
                table: "Provincia");

            migrationBuilder.DropIndex(
                name: "IX_Localidad_ProvinciaId",
                table: "Localidad");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Provincia");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Localidad");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Localidad");
        }
    }
}
