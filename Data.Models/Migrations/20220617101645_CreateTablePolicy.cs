using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTablePolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    idPolicy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Код на Policy"),
                    PolicyDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Описание на Policy")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.idPolicy);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Policy_PolicyCode",
                table: "Policy",
                column: "PolicyCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Policy");
        }
    }
}
