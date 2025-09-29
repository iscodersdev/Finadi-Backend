using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig55 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreadoPorUsuarioId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacionModificacion",
                table: "Personas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreadoPorUsuarioId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacionModificacion",
                table: "Clientes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadoPorUsuarioId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "FechaCreacionModificacion",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "CreadoPorUsuarioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaCreacionModificacion",
                table: "Clientes");
        }
    }
}
