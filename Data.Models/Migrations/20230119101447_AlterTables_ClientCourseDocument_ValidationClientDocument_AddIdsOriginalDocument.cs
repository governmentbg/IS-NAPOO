using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_ClientCourseDocument_ValidationClientDocument_AddIdsOriginalDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с оригиналния документ от курс за валидиране");

            migrationBuilder.AddColumn<int>(
                name: "IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с оригиналния документ от курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientDocument_IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument",
                column: "IdOriginalValidationClientDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocument_IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument",
                column: "IdOriginalClientCourseDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourseDocument_Training_ClientCourseDocument_IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument",
                column: "IdOriginalClientCourseDocument",
                principalTable: "Training_ClientCourseDocument",
                principalColumn: "IdClientCourseDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationClientDocument_IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument",
                column: "IdOriginalValidationClientDocument",
                principalTable: "Training_ValidationClientDocument",
                principalColumn: "IdValidationClientDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourseDocument_Training_ClientCourseDocument_IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationClientDocument_IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ValidationClientDocument_IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ClientCourseDocument_IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropColumn(
                name: "IdOriginalValidationClientDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropColumn(
                name: "IdOriginalClientCourseDocument",
                table: "Training_ClientCourseDocument");
        }
    }
}
