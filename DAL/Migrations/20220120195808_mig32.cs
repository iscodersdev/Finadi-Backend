using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CUIT",
                table: "Organismos",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "Organismos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domicilio",
                table: "Organismos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalidadId",
                table: "Organismos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "Organismos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Organismos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_LocalidadId",
                table: "Organismos",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_ProvinciaId",
                table: "Organismos",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organismos_Localidad_LocalidadId",
                table: "Organismos",
                column: "LocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organismos_Provincia_ProvinciaId",
                table: "Organismos",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddColumn<string>(
                name: "CapitalInversorEnLetras",
                table: "Prestamos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organismos_Localidad_LocalidadId",
                table: "Organismos");

            migrationBuilder.DropForeignKey(
                name: "FK_Organismos_Provincia_ProvinciaId",
                table: "Organismos");

            migrationBuilder.DropIndex(
                name: "IX_Organismos_LocalidadId",
                table: "Organismos");

            migrationBuilder.DropIndex(
                name: "IX_Organismos_ProvinciaId",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "CUIT",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "Domicilio",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "LocalidadId",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "CapitalInversorEnLetras",
                table: "Prestamos");
        }
    }
}
