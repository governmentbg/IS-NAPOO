using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingCurriculumChangeIdCandidateCurriculumAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Curriculum_Candidate_Curriculum_IdCandidateCurriculum",
                table: "Training_Curriculum");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateCurriculum",
                table: "Training_Curriculum",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Curriculum_Candidate_Curriculum_IdCandidateCurriculum",
                table: "Training_Curriculum",
                column: "IdCandidateCurriculum",
                principalTable: "Candidate_Curriculum",
                principalColumn: "IdCandidateCurriculum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Curriculum_Candidate_Curriculum_IdCandidateCurriculum",
                table: "Training_Curriculum");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateCurriculum",
                table: "Training_Curriculum",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Curriculum_Candidate_Curriculum_IdCandidateCurriculum",
                table: "Training_Curriculum",
                column: "IdCandidateCurriculum",
                principalTable: "Candidate_Curriculum",
                principalColumn: "IdCandidateCurriculum",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
