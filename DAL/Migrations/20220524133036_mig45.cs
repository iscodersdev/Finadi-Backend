using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig45 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPresentacionDeInversor",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Altura",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaPresentacionDeInversor",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "Altura",
                table: "Clientes");
        }
    }
}
