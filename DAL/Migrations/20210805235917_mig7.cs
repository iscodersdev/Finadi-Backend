using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendedorId",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_VendedorId",
                table: "Prestamos",
                column: "VendedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Vendedores_VendedorId",
                table: "Prestamos",
                column: "VendedorId",
                principalTable: "Vendedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Vendedores_VendedorId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_VendedorId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "VendedorId",
                table: "Prestamos");
        }
    }
}
