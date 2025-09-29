using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProvinciaId",
                table: "Prestamos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PaisId",
                table: "Prestamos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LocalidadId",
                table: "Prestamos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_LocalidadId",
                table: "Prestamos",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_PaisId",
                table: "Prestamos",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_ProvinciaId",
                table: "Prestamos",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Localidad_LocalidadId",
                table: "Prestamos",
                column: "LocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Paises_PaisId",
                table: "Prestamos",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Provincia_ProvinciaId",
                table: "Prestamos",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Localidad_LocalidadId",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Paises_PaisId",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Provincia_ProvinciaId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_LocalidadId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_PaisId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_ProvinciaId",
                table: "Prestamos");

            migrationBuilder.AlterColumn<int>(
                name: "ProvinciaId",
                table: "Prestamos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaisId",
                table: "Prestamos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocalidadId",
                table: "Prestamos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
