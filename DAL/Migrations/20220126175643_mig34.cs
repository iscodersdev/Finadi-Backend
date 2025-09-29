using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CFTSinImpuesto",
                table: "Prestamos");

            migrationBuilder.AddColumn<int>(
                name: "CFTSinImpuestoId",
                table: "Inversores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CFTSinImpuesto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFTSinImpuesto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_CFTSinImpuestoId",
                table: "Inversores",
                column: "CFTSinImpuestoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inversores_CFTSinImpuesto_CFTSinImpuestoId",
                table: "Inversores",
                column: "CFTSinImpuestoId",
                principalTable: "CFTSinImpuesto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inversores_CFTSinImpuesto_CFTSinImpuestoId",
                table: "Inversores");

            migrationBuilder.DropTable(
                name: "CFTSinImpuesto");

            migrationBuilder.DropIndex(
                name: "IX_Inversores_CFTSinImpuestoId",
                table: "Inversores");

            migrationBuilder.DropColumn(
                name: "CFTSinImpuestoId",
                table: "Inversores");

            migrationBuilder.AddColumn<decimal>(
                name: "CFTSinImpuesto",
                table: "Prestamos",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
