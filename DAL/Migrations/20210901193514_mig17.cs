using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FotoCertificadoDescuento",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoReciboHaber",
                table: "Prestamos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoCertificadoDescuento",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "FotoReciboHaber",
                table: "Prestamos");
        }
    }
}
