using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InversorId",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCapitalInversor",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_InversorId",
                table: "Prestamos",
                column: "InversorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Inversores_InversorId",
                table: "Prestamos",
                column: "InversorId",
                principalTable: "Inversores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Inversores_InversorId",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_InversorId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "InversorId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TotalCapitalInversor",
                table: "Prestamos");
        }
    }
}
