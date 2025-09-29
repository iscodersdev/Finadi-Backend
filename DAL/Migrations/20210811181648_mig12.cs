using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoDNIAnverso",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "FotoDNIReverso",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "FotoSosteniendoDNI",
                table: "Prestamos");

            migrationBuilder.AddColumn<byte[]>(
                name: "FirmaOlografica",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FirmaOlograficaConfirmacion",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoDNIAnverso",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoDNIReverso",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoSosteniendoDNI",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "LegajoElectronico",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmaOlografica",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FirmaOlograficaConfirmacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FotoDNIAnverso",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FotoDNIReverso",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FotoSosteniendoDNI",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "LegajoElectronico",
                table: "Clientes");

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoDNIAnverso",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoDNIReverso",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoSosteniendoDNI",
                table: "Prestamos",
                nullable: true);
        }
    }
}
