using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableSpecialityNKPD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPPOO_SpecialityNKPD",
                columns: table => new
                {
                    IdSpecialityNKPD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdNKPD = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_SpecialityNKPD", x => x.IdSpecialityNKPD);
                    table.ForeignKey(
                        name: "FK_SPPOO_SpecialityNKPD_DOC_NKPD_IdNKPD",
                        column: x => x.IdNKPD,
                        principalTable: "DOC_NKPD",
                        principalColumn: "IdNKPD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPPOO_SpecialityNKPD_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_SpecialityNKPD_IdNKPD",
                table: "SPPOO_SpecialityNKPD",
                column: "IdNKPD");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_SpecialityNKPD_IdSpeciality",
                table: "SPPOO_SpecialityNKPD",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_SpecialityNKPD");
        }
    }
}
