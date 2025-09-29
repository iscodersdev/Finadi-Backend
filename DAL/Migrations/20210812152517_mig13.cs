using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Financiable",
                table: "Productos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Oferta",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinanciacionProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: true),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    InteresesPorCuota = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanciacionProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanciacionProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanciacionProductos_ProductoId",
                table: "FinanciacionProductos",
                column: "ProductoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanciacionProductos");

            migrationBuilder.DropColumn(
                name: "Financiable",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Oferta",
                table: "Productos");
        }
    }
}
