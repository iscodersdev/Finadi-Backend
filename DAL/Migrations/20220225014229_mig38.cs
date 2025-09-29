using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig38 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LugarNacimientoId",
                table: "Personas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_LugarNacimientoId",
                table: "Personas",
                column: "LugarNacimientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Provincia_LugarNacimientoId",
                table: "Personas",
                column: "LugarNacimientoId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Provincia_LugarNacimientoId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_LugarNacimientoId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "LugarNacimientoId",
                table: "Personas");
        }
    }
}
