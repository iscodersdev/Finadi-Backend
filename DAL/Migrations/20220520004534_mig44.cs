using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig44 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCargaId",
                table: "LineasPrestamosPlanes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosPlanes_UsuarioCargaId",
                table: "LineasPrestamosPlanes",
                column: "UsuarioCargaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineasPrestamosPlanes_AspNetUsers_UsuarioCargaId",
                table: "LineasPrestamosPlanes",
                column: "UsuarioCargaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineasPrestamosPlanes_AspNetUsers_UsuarioCargaId",
                table: "LineasPrestamosPlanes");

            migrationBuilder.DropIndex(
                name: "IX_LineasPrestamosPlanes_UsuarioCargaId",
                table: "LineasPrestamosPlanes");

            migrationBuilder.DropColumn(
                name: "UsuarioCargaId",
                table: "LineasPrestamosPlanes");
        }
    }
}
