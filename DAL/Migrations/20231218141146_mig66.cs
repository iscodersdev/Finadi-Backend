using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig66 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Localidad_DomicilioLocalidadId",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Paises_DomicilioPaisId",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Provincia_DomicilioProvinciaId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_DomicilioLocalidadId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_DomicilioPaisId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_DomicilioProvinciaId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "DomicilioLocalidadId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "DomicilioPaisId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "DomicilioProvinciaId",
                table: "Prestamos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DomicilioLocalidadId",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DomicilioPaisId",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DomicilioProvinciaId",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_DomicilioLocalidadId",
                table: "Prestamos",
                column: "DomicilioLocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_DomicilioPaisId",
                table: "Prestamos",
                column: "DomicilioPaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_DomicilioProvinciaId",
                table: "Prestamos",
                column: "DomicilioProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Localidad_DomicilioLocalidadId",
                table: "Prestamos",
                column: "DomicilioLocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Paises_DomicilioPaisId",
                table: "Prestamos",
                column: "DomicilioPaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Provincia_DomicilioProvinciaId",
                table: "Prestamos",
                column: "DomicilioProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
