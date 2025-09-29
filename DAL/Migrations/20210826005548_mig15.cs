using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineasPrestamosTiposPersonas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineaPrestamoId = table.Column<int>(nullable: true),
                    TipoPersonaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasPrestamosTiposPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasPrestamosTiposPersonas_LineasPrestamos_LineaPrestamoId",
                        column: x => x.LineaPrestamoId,
                        principalTable: "LineasPrestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineasPrestamosTiposPersonas_TiposPersonas_TipoPersonaId",
                        column: x => x.TipoPersonaId,
                        principalTable: "TiposPersonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosTiposPersonas_LineaPrestamoId",
                table: "LineasPrestamosTiposPersonas",
                column: "LineaPrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosTiposPersonas_TipoPersonaId",
                table: "LineasPrestamosTiposPersonas",
                column: "TipoPersonaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasPrestamosTiposPersonas");
        }
    }
}
