using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_CourseProtocol_AddColumn_IdCourseCommissionMember_ValidationProtocol_AddColumn_IdValidationCommissionMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdValidationCommissionMember",
                table: "Training_ValidationProtocol",
                type: "int",
                nullable: true,
                comment: "Връзка с член на изпитна комисия към курс за валидиране");

            migrationBuilder.AddColumn<int>(
                name: "IdCourseCommissionMember",
                table: "Training_CourseProtocol",
                type: "int",
                nullable: true,
                comment: "Връзка с член на изпитна комисия към курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationProtocol_IdValidationCommissionMember",
                table: "Training_ValidationProtocol",
                column: "IdValidationCommissionMember");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseProtocol_IdCourseCommissionMember",
                table: "Training_CourseProtocol",
                column: "IdCourseCommissionMember");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseProtocol_Training_CourseCommissionMember_IdCourseCommissionMember",
                table: "Training_CourseProtocol",
                column: "IdCourseCommissionMember",
                principalTable: "Training_CourseCommissionMember",
                principalColumn: "IdCourseCommissionMember");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationProtocol_Training_ValidationCommissionMember_IdValidationCommissionMember",
                table: "Training_ValidationProtocol",
                column: "IdValidationCommissionMember",
                principalTable: "Training_ValidationCommissionMember",
                principalColumn: "IdValidationCommissionMember");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseProtocol_Training_CourseCommissionMember_IdCourseCommissionMember",
                table: "Training_CourseProtocol");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationProtocol_Training_ValidationCommissionMember_IdValidationCommissionMember",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropIndex(
                name: "IX_Training_ValidationProtocol_IdValidationCommissionMember",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropIndex(
                name: "IX_Training_CourseProtocol_IdCourseCommissionMember",
                table: "Training_CourseProtocol");

            migrationBuilder.DropColumn(
                name: "IdValidationCommissionMember",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropColumn(
                name: "IdCourseCommissionMember",
                table: "Training_CourseProtocol");
        }
    }
}
