using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableERUSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_ERUSpeciality",
                columns: table => new
                {
                    IdERUSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdERU = table.Column<int>(type: "int", nullable: false),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_ERUSpeciality", x => x.IdERUSpeciality);
                    table.ForeignKey(
                        name: "FK_DOC_ERUSpeciality_DOC_ERU_IdERU",
                        column: x => x.IdERU,
                        principalTable: "DOC_ERU",
                        principalColumn: "IdERU",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOC_ERUSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_ERUSpeciality_IdERU",
                table: "DOC_ERUSpeciality",
                column: "IdERU");

            migrationBuilder.CreateIndex(
                name: "IX_DOC_ERUSpeciality_IdSpeciality",
                table: "DOC_ERUSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_ERUSpeciality");
        }
    }
}
