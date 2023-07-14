using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_FollowUpControlDocument_CourseSubjectGrade_ValidationClientDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Training_CourseSubjectGrade");

            migrationBuilder.AddColumn<int>(
                name: "IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с тип докумет към печатница на МОН");

            migrationBuilder.AddColumn<double>(
                name: "PracticeGrade",
                table: "Training_CourseSubjectGrade",
                type: "float",
                nullable: true,
                comment: "Оценка по предмет за практика");

            migrationBuilder.AddColumn<double>(
                name: "TheoryGrade",
                table: "Training_CourseSubjectGrade",
                type: "float",
                nullable: true,
                comment: "Оценка по предмет за теория");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Control_FollowUpControlDocument",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Прикачен файл от център");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientDocument_IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument",
                column: "IdTypeOfRequestedDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClientDocument_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument",
                column: "IdTypeOfRequestedDocument",
                principalTable: "Request_TypeOfRequestedDocument",
                principalColumn: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClientDocument_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ValidationClientDocument_IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropColumn(
                name: "IdTypeOfRequestedDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropColumn(
                name: "PracticeGrade",
                table: "Training_CourseSubjectGrade");

            migrationBuilder.DropColumn(
                name: "TheoryGrade",
                table: "Training_CourseSubjectGrade");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Control_FollowUpControlDocument");

            migrationBuilder.AddColumn<double>(
                name: "Grade",
                table: "Training_CourseSubjectGrade",
                type: "float",
                nullable: true,
                comment: "Обща оценка по предмет от учебен план");
        }
    }
}
