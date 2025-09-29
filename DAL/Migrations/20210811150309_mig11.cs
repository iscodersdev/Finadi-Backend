using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Administradores",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PersonasId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonasId",
                table: "AspNetUsers",
                column: "PersonasId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Personas_PersonasId",
                table: "AspNetUsers",
                column: "PersonasId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Personas_PersonasId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonasId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Administradores",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonasId",
                table: "AspNetUsers");
        }
    }
}
