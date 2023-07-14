using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableMenuNodeRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuNodeRole",
                columns: table => new
                {
                    IdMenuNodeRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdApplicationRole = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Връзка с Роля"),
                    IdMenuNode = table.Column<int>(type: "int", nullable: false, comment: "Връзка с елемент от менюто")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuNodeRole", x => x.IdMenuNodeRole);
                    table.ForeignKey(
                        name: "FK_MenuNodeRole_AspNetRoles_IdApplicationRole",
                        column: x => x.IdApplicationRole,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuNodeRole_MenuNode_IdMenuNode",
                        column: x => x.IdMenuNode,
                        principalTable: "MenuNode",
                        principalColumn: "IdMenuNode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuNodeRole_IdApplicationRole",
                table: "MenuNodeRole",
                column: "IdApplicationRole");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNodeRole_IdMenuNode",
                table: "MenuNodeRole",
                column: "IdMenuNode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuNodeRole");
        }
    }
}
