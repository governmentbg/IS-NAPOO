using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseProtocol_AddColumn_IdCandidateProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCandidateProvider",
                table: "Training_CourseProtocol",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с ЦПО");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseProtocol_IdCandidateProvider",
                table: "Training_CourseProtocol",
                column: "IdCandidateProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseProtocol_Candidate_Provider_IdCandidateProvider",
                table: "Training_CourseProtocol",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseProtocol_Candidate_Provider_IdCandidateProvider",
                table: "Training_CourseProtocol");

            migrationBuilder.DropIndex(
                name: "IX_Training_CourseProtocol_IdCandidateProvider",
                table: "Training_CourseProtocol");

            migrationBuilder.DropColumn(
                name: "IdCandidateProvider",
                table: "Training_CourseProtocol");
        }
    }
}
