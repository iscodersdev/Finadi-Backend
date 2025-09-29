using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CFTSinImpuesto",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TEAId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TNAId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TasasEfectivaAnual",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasEfectivaAnual", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TasasNominalAnual",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasNominalAnual", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TEAId",
                table: "Inversores",
                column: "TEAId");

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TNAId",
                table: "Inversores",
                column: "TNAId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_TasasEfectivaAnual_TEAId",
                table: "Inversores",
                column: "TEAId",
                principalTable: "TasasEfectivaAnual",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_TasasNominalAnual_TNAId",
                table: "Inversores",
                column: "TNAId",
                principalTable: "TasasNominalAnual",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_TasasEfectivaAnual_TEAId",
                table: "Inversores");

            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_TasasNominalAnual_TNAId",
                table: "Inversores");

            migrationBuilder.DropTable(
                name: "TasasEfectivaAnual");

            migrationBuilder.DropTable(
                name: "TasasNominalAnual");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_TEAId",
                table: "Inversores");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_TNAId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "CFTSinImpuesto",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "TEAId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "TNAId",
                table: "Inversores");
        }
    }
}
