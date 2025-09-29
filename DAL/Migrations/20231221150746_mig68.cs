using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig68 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Calle",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalleNro",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodPostal",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dpto",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Piso",
                table: "Prestamos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calle",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "CalleNro",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "CodPostal",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "Dpto",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "Piso",
                table: "Prestamos");
        }
    }
}
