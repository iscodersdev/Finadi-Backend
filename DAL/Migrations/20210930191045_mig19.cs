using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenciaAId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferenciaBId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Referencias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NombreCompleto = table.Column<string>(nullable: true),
                    Vinculo = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referencias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ReferenciaAId",
                table: "Clientes",
                column: "ReferenciaAId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ReferenciaBId",
                table: "Clientes",
                column: "ReferenciaBId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Referencias_ReferenciaAId",
                table: "Clientes",
                column: "ReferenciaAId",
                principalTable: "Referencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Referencias_ReferenciaBId",
                table: "Clientes",
                column: "ReferenciaBId",
                principalTable: "Referencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Referencias_ReferenciaAId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Referencias_ReferenciaBId",
                table: "Clientes");

            migrationBuilder.DropTable(
                name: "Referencias");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_ReferenciaAId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_ReferenciaBId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ReferenciaAId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ReferenciaBId",
                table: "Clientes");
        }
    }
}
