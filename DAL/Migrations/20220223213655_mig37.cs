using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig37 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CFTConImpuestoId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TEMId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CFTConImpuesto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFTConImpuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TEM",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEM", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_CFTConImpuestoId",
                table: "Inversores",
                column: "CFTConImpuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TEMId",
                table: "Inversores",
                column: "TEMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_CFTConImpuesto_CFTConImpuestoId",
                table: "Inversores",
                column: "CFTConImpuestoId",
                principalTable: "CFTConImpuesto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_TEM_TEMId",
                table: "Inversores",
                column: "TEMId",
                principalTable: "TEM",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_CFTConImpuesto_CFTConImpuestoId",
                table: "Inversores");

            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_TEM_TEMId",
                table: "Inversores");

            migrationBuilder.DropTable(
                name: "CFTConImpuesto");

            migrationBuilder.DropTable(
                name: "TEM");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_CFTConImpuestoId",
                table: "Inversores");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_TEMId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "CFTConImpuestoId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "TEMId",
                table: "Inversores");
        }
    }
}
