using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableUserAnswerOpen_UserAnswer_PointsDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Points",
                table: "Assess_UserAnswerOpen",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Точки");

            migrationBuilder.AlterColumn<decimal>(
                name: "Points",
                table: "Assess_UserAnswer",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Точки");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                column: "IdCandidateCurriculumModification",
                principalTable: "Candidate_CurriculumModification",
                principalColumn: "IdCandidateCurriculumModification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "Assess_UserAnswerOpen",
                type: "int",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "Точки");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "Assess_UserAnswer",
                type: "int",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "Точки");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                column: "IdCandidateCurriculumModification",
                principalTable: "Candidate_CurriculumModification",
                principalColumn: "IdCandidateCurriculumModification",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
