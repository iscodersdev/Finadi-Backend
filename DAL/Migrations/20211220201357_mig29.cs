using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig29 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organismos_CuotasSociales_CuotaId",
                table: "Organismos");

            migrationBuilder.DropIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "CuotaId",
                table: "Organismos");

            migrationBuilder.RenameColumn(
                name: "Organismo",
                table: "CuotasSociales",
                newName: "TipoPersona");

            migrationBuilder.AddColumn<int>(
                name: "CuotaId",
                table: "TiposPersonas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPersonas_CuotaId",
                table: "TiposPersonas",
                column: "CuotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TiposPersonas_CuotasSociales_CuotaId",
                table: "TiposPersonas",
                column: "CuotaId",
                principalTable: "CuotasSociales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiposPersonas_CuotasSociales_CuotaId",
                table: "TiposPersonas");

            migrationBuilder.DropIndex(
                name: "IX_TiposPersonas_CuotaId",
                table: "TiposPersonas");

            migrationBuilder.DropColumn(
                name: "CuotaId",
                table: "TiposPersonas");

            migrationBuilder.RenameColumn(
                name: "TipoPersona",
                table: "CuotasSociales",
                newName: "Organismo");

            migrationBuilder.AddColumn<int>(
                name: "CuotaId",
                table: "Organismos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos",
                column: "CuotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organismos_CuotasSociales_CuotaId",
                table: "Organismos",
                column: "CuotaId",
                principalTable: "CuotasSociales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
