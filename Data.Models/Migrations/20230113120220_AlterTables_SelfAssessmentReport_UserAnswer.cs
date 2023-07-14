using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_SelfAssessmentReport_UserAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAnswer",
                table: "Assess_UserAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с  отговор");

            migrationBuilder.AddColumn<int>(
                name: "IdSurveyResult",
                table: "Arch_SelfAssessmentReport",
                type: "int",
                nullable: true,
                comment: "Връзка с анкета");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_UserAnswer_IdAnswer",
                table: "Assess_UserAnswer",
                column: "IdAnswer");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_SelfAssessmentReport_IdSurveyResult",
                table: "Arch_SelfAssessmentReport",
                column: "IdSurveyResult");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_SelfAssessmentReport_Assess_SurveyResult_IdSurveyResult",
                table: "Arch_SelfAssessmentReport",
                column: "IdSurveyResult",
                principalTable: "Assess_SurveyResult",
                principalColumn: "IdSurveyResult");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer",
                column: "IdAnswer",
                principalTable: "Assess_Answer",
                principalColumn: "IdAnswer",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_SelfAssessmentReport_Assess_SurveyResult_IdSurveyResult",
                table: "Arch_SelfAssessmentReport");

            migrationBuilder.DropForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer");

            migrationBuilder.DropIndex(
                name: "IX_Assess_UserAnswer_IdAnswer",
                table: "Assess_UserAnswer");

            migrationBuilder.DropIndex(
                name: "IX_Arch_SelfAssessmentReport_IdSurveyResult",
                table: "Arch_SelfAssessmentReport");

            migrationBuilder.DropColumn(
                name: "IdAnswer",
                table: "Assess_UserAnswer");

            migrationBuilder.DropColumn(
                name: "IdSurveyResult",
                table: "Arch_SelfAssessmentReport");
        }
    }
}
