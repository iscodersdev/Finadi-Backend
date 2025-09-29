using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig61 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ConstanciaCBU",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCelularDetectado",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstanciaCBU",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "NumeroCelularDetectado",
                table: "Clientes");
        }
    }
}
