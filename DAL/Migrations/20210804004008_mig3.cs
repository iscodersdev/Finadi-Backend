using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Personas_PersonaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_EstadosCiviles_EstadoCivilId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Paises_PaisId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TipoDocumento_TipoDocumentoId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TiposPersonas_TipoPersonaId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Genero_GeneroID",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TipoDocumento_TipoDocumentoId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_AspNetUsers_UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_EstadoCivilId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoDocumentoId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CUIL",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CantidadHijos",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EstadoCivilId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Nombres",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "NumeroDocumento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoId",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "GeneroID",
                table: "Personas",
                newName: "GeneroId");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Personas",
                newName: "Nombres");

            migrationBuilder.RenameIndex(
                name: "IX_Personas_GeneroID",
                table: "Personas",
                newName: "IX_Personas_GeneroId");

            migrationBuilder.RenameColumn(
                name: "TipoPersonaId",
                table: "Clientes",
                newName: "PersonaId");

            migrationBuilder.RenameColumn(
                name: "RecordarPassword",
                table: "Clientes",
                newName: "ClienteValidado");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_TipoPersonaId",
                table: "Clientes",
                newName: "IX_Clientes_PersonaId");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                table: "AspNetUsers",
                newName: "VendedoresId");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "AspNetUsers",
                newName: "ProveedorId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_PersonaId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_VendedoresId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_EmpresaId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ProveedorId");

            migrationBuilder.AddColumn<int>(
                name: "OrganismoId",
                table: "TiposPersonas",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipoDocumentoId",
                table: "Personas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GeneroId",
                table: "Personas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CantidadHijos",
                table: "Personas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoCivilId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPersonaId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RecordarPassword",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPersonas_OrganismoId",
                table: "TiposPersonas",
                column: "OrganismoId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_PaisId",
                table: "Personas",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoPersonaId",
                table: "Personas",
                column: "TipoPersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Proveedores_ProveedorId",
                table: "AspNetUsers",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Vendedores_VendedoresId",
                table: "AspNetUsers",
                column: "VendedoresId",
                principalTable: "Vendedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Personas_PersonaId",
                table: "Clientes",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_EstadosCiviles_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId",
                principalTable: "EstadosCiviles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Genero_GeneroId",
                table: "Personas",
                column: "GeneroId",
                principalTable: "Genero",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Paises_PaisId",
                table: "Personas",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TipoDocumento_TipoDocumentoId",
                table: "Personas",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TiposPersonas_TipoPersonaId",
                table: "Personas",
                column: "TipoPersonaId",
                principalTable: "TiposPersonas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposPersonas_Organismos_OrganismoId",
                table: "TiposPersonas",
                column: "OrganismoId",
                principalTable: "Organismos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Proveedores_ProveedorId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Vendedores_VendedoresId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Personas_PersonaId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_EstadosCiviles_EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Genero_GeneroId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Paises_PaisId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TipoDocumento_TipoDocumentoId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TiposPersonas_TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposPersonas_Organismos_OrganismoId",
                table: "TiposPersonas");

            migrationBuilder.DropIndex(
                name: "IX_TiposPersonas_OrganismoId",
                table: "TiposPersonas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_PaisId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "OrganismoId",
                table: "TiposPersonas");

            migrationBuilder.DropColumn(
                name: "CantidadHijos",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RecordarPassword",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "GeneroId",
                table: "Personas",
                newName: "GeneroID");

            migrationBuilder.RenameColumn(
                name: "Nombres",
                table: "Personas",
                newName: "Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Personas_GeneroId",
                table: "Personas",
                newName: "IX_Personas_GeneroID");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                table: "Clientes",
                newName: "TipoPersonaId");

            migrationBuilder.RenameColumn(
                name: "ClienteValidado",
                table: "Clientes",
                newName: "RecordarPassword");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_PersonaId",
                table: "Clientes",
                newName: "IX_Clientes_TipoPersonaId");

            migrationBuilder.RenameColumn(
                name: "VendedoresId",
                table: "AspNetUsers",
                newName: "PersonaId");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                table: "AspNetUsers",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_VendedoresId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ProveedorId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_EmpresaId");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Proveedores",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipoDocumentoId",
                table: "Personas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GeneroID",
                table: "Personas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CUIL",
                table: "Clientes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "CantidadHijos",
                table: "Clientes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoCivilId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Clientes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombres",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NumeroDocumento",
                table: "Clientes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_UsuarioId",
                table: "Proveedores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EstadoCivilId",
                table: "Clientes",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoDocumentoId",
                table: "Clientes",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Personas_PersonaId",
                table: "AspNetUsers",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_EstadosCiviles_EstadoCivilId",
                table: "Clientes",
                column: "EstadoCivilId",
                principalTable: "EstadosCiviles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Paises_PaisId",
                table: "Clientes",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TipoDocumento_TipoDocumentoId",
                table: "Clientes",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TiposPersonas_TipoPersonaId",
                table: "Clientes",
                column: "TipoPersonaId",
                principalTable: "TiposPersonas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Genero_GeneroID",
                table: "Personas",
                column: "GeneroID",
                principalTable: "Genero",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TipoDocumento_TipoDocumentoId",
                table: "Personas",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_AspNetUsers_UsuarioId",
                table: "Proveedores",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
