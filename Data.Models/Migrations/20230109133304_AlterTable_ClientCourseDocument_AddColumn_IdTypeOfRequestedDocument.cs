using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ClientCourseDocument_AddColumn_IdTypeOfRequestedDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с тип докумет към печатница на МОН");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocument_IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument",
                column: "IdTypeOfRequestedDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourseDocument_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument",
                column: "IdTypeOfRequestedDocument",
                principalTable: "Request_TypeOfRequestedDocument",
                principalColumn: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourseDocument_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ClientCourseDocument_IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropColumn(
                name: "IdTypeOfRequestedDocument",
                table: "Training_ClientCourseDocument");
        }
    }
}
