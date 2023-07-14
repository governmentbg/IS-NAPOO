using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class DropDocSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_DocSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_DocSpeciality",
                columns: table => new
                {
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdDOC = table.Column<int>(type: "int", nullable: false),
                    IdDocSpeciality = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_DocSpeciality", x => new { x.IdSpeciality, x.IdDOC });
                    table.ForeignKey(
                        name: "FK_DOC_DocSpeciality_DOC_DOC_IdDOC",
                        column: x => x.IdDOC,
                        principalTable: "DOC_DOC",
                        principalColumn: "IdDOC",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOC_DocSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_DocSpeciality_IdDOC",
                table: "DOC_DocSpeciality",
                column: "IdDOC");
        }
    }
}
