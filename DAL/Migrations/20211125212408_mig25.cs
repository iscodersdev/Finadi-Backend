using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CuotaId",
                table: "Organismos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComprasProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    ProductoId = table.Column<int>(nullable: true),
                    MovimientoId = table.Column<int>(nullable: true),
                    PrestamoId = table.Column<int>(nullable: true),
                    FechaCompra = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<int>(nullable: false),
                    TipoCompra = table.Column<int>(nullable: false),
                    FechaAnulacion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_MovimientosBilletera_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "MovimientosBilletera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuotasSociales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ValorCuota = table.Column<decimal>(nullable: false),
                    ImpusoCuota = table.Column<string>(nullable: true),
                    Organismo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuotasSociales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inversores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    Tasa = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inversores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos",
                column: "CuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_ClienteId",
                table: "ComprasProductos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_MovimientoId",
                table: "ComprasProductos",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_PrestamoId",
                table: "ComprasProductos",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_ProductoId",
                table: "ComprasProductos",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organismos_CuotasSociales_CuotaId",
                table: "Organismos",
                column: "CuotaId",
                principalTable: "CuotasSociales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organismos_CuotasSociales_CuotaId",
                table: "Organismos");

            migrationBuilder.DropTable(
                name: "ComprasProductos");

            migrationBuilder.DropTable(
                name: "CuotasSociales");

            migrationBuilder.DropTable(
                name: "Inversores");

            migrationBuilder.DropIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos");

            migrationBuilder.DropColumn(
                name: "CuotaId",
                table: "Organismos");
        }
    }
}
