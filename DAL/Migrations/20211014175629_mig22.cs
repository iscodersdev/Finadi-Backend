using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "AdjuntoTransferencia",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtensionAdjuntoTransferencia",
                table: "Prestamos",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CabeceraMail",
                table: "Empresas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CasillaMail",
                table: "Empresas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MailBienvenida",
                table: "Empresas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordMail",
                table: "Empresas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuertoMail",
                table: "Empresas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SSLMail",
                table: "Empresas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UrlMail",
                table: "Empresas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsernameMail",
                table: "Empresas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjuntoTransferencia",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "ExtensionAdjuntoTransferencia",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "CabeceraMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CasillaMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "MailBienvenida",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PasswordMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PuertoMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "SSLMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "UrlMail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "UsernameMail",
                table: "Empresas");
        }
    }
}
