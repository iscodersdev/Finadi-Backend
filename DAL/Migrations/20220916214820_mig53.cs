using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig53 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CapitalAmprom",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CapitalFINADI",
                table: "LineasPrestamosPlanes",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapitalAmprom",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "CapitalFINADI",
                table: "LineasPrestamosPlanes");
        }
    }
}
