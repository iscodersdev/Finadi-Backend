using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig52 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TNAAmprom",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TNA",
                table: "LineasPrestamosPlanes",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TNAAmprom",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TNA",
                table: "LineasPrestamosPlanes");
        }
    }
}
