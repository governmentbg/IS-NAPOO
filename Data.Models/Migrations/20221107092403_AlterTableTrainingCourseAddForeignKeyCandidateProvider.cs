using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingCourseAddForeignKeyCandidateProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCandidateProvider",
                table: "Training_Course",
                type: "int",
                nullable: true,
                comment: "Връзка с CandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Course_IdCandidateProvider",
                table: "Training_Course",
                column: "IdCandidateProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Course_Candidate_Provider_IdCandidateProvider",
                table: "Training_Course",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Course_Candidate_Provider_IdCandidateProvider",
                table: "Training_Course");

            migrationBuilder.DropIndex(
                name: "IX_Training_Course_IdCandidateProvider",
                table: "Training_Course");

            migrationBuilder.DropColumn(
                name: "IdCandidateProvider",
                table: "Training_Course");
        }
    }
}
