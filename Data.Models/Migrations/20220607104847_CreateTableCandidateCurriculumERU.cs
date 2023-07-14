using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateCurriculumERU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_CurriculumERU",
                columns: table => new
                {
                    IdCandidateCurriculumERU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdERU = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_CurriculumERU", x => x.IdCandidateCurriculumERU);
                    table.ForeignKey(
                        name: "FK_Candidate_CurriculumERU_Candidate_Curriculum_IdCandidateCurriculum",
                        column: x => x.IdCandidateCurriculum,
                        principalTable: "Candidate_Curriculum",
                        principalColumn: "IdCandidateCurriculum",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Candidate_CurriculumERU_DOC_ERU_IdERU",
                        column: x => x.IdERU,
                        principalTable: "DOC_ERU",
                        principalColumn: "IdERU",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_CurriculumERU_IdCandidateCurriculum",
                table: "Candidate_CurriculumERU",
                column: "IdCandidateCurriculum");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_CurriculumERU_IdERU",
                table: "Candidate_CurriculumERU",
                column: "IdERU");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_CurriculumERU");
        }
    }
}
