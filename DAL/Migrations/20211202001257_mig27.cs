using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tasa",
                table: "Inversores");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Inversores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TasaActualId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TasasInversores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasInversores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TasaActualId",
                table: "Inversores",
                column: "TasaActualId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_TasasInversores_TasaActualId",
                table: "Inversores",
                column: "TasaActualId",
                principalTable: "TasasInversores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_TasasInversores_TasaActualId",
                table: "Inversores");

            migrationBuilder.DropTable(
                name: "TasasInversores");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_TasaActualId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "TasaActualId",
                table: "Inversores");

            migrationBuilder.AddColumn<decimal>(
                name: "Tasa",
                table: "Inversores",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
