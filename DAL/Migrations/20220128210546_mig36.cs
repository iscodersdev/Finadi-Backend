using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig36 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CFTSinImpuesto",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TEA",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TNA",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TasaInversor",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CFTSinImpuesto",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TEA",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TNA",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TasaInversor",
                table: "Prestamos");
        }
    }
}
