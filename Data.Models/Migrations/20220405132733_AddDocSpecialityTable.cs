using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddDocSpecialityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_DocSpeciality",
                columns: table => new
                {
                    IdDOC = table.Column<int>(type: "int", nullable: false),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdDocSpeciality = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_DocSpeciality", x => new { x.IdSpeciality, x.IdDOC });
                    table.ForeignKey(
                        name: "FK_DOC_DocSpeciality_DOC_DOC_IdDOC",
                        column: x => x.IdDOC,
                        principalTable: "DOC_DOC",
                        principalColumn: "IdDOC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOC_DocSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_DocSpeciality_IdDOC",
                table: "DOC_DocSpeciality",
                column: "IdDOC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_DocSpeciality");
        }
    }
}
