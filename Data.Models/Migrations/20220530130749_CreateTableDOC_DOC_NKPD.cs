using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableDOC_DOC_NKPD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_DOC_NKPD",
                columns: table => new
                {
                    IdDOC_DOC_NKPD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDOC = table.Column<int>(type: "int", nullable: false),
                    IdNKPD = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_DOC_NKPD", x => x.IdDOC_DOC_NKPD);
                    table.ForeignKey(
                        name: "FK_DOC_DOC_NKPD_DOC_DOC_IdDOC",
                        column: x => x.IdDOC,
                        principalTable: "DOC_DOC",
                        principalColumn: "IdDOC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOC_DOC_NKPD_DOC_NKPD_IdNKPD",
                        column: x => x.IdNKPD,
                        principalTable: "DOC_NKPD",
                        principalColumn: "IdNKPD",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_DOC_NKPD_IdDOC",
                table: "DOC_DOC_NKPD",
                column: "IdDOC");

            migrationBuilder.CreateIndex(
                name: "IX_DOC_DOC_NKPD_IdNKPD",
                table: "DOC_DOC_NKPD",
                column: "IdNKPD");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_DOC_NKPD");
        }
    }
}
