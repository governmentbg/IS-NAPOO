using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableTrainingCurriculumERU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CurriculumERU",
                columns: table => new
                {
                    IdTrainingCurriculumERU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTrainingCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdERU = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CurriculumERU", x => x.IdTrainingCurriculumERU);
                    table.ForeignKey(
                        name: "FK_Training_CurriculumERU_DOC_ERU_IdERU",
                        column: x => x.IdERU,
                        principalTable: "DOC_ERU",
                        principalColumn: "IdERU",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_CurriculumERU_Training_Curriculum_IdTrainingCurriculum",
                        column: x => x.IdTrainingCurriculum,
                        principalTable: "Training_Curriculum",
                        principalColumn: "IdTrainingCurriculum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_CurriculumERU_IdERU",
                table: "Training_CurriculumERU",
                column: "IdERU");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CurriculumERU_IdTrainingCurriculum",
                table: "Training_CurriculumERU",
                column: "IdTrainingCurriculum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CurriculumERU");
        }
    }
}
