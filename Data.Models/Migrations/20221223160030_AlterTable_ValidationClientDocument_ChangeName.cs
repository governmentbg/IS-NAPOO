using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationClientDocument_ChangeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationCourseDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ValidationCourseDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationCourseDocument_Training_ValidationClient_IdValidationClient",
                table: "Training_ValidationCourseDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationCourseDocument_Training_ValidationProtocol_IdValidationProtocol",
                table: "Training_ValidationCourseDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Training_ValidationCourseDocument",
                table: "Training_ValidationCourseDocument");

            migrationBuilder.RenameTable(
                name: "Training_ValidationCourseDocument",
                newName: "Training_ValidationClientDocument");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationCourseDocument_IdValidationProtocol",
                table: "Training_ValidationClientDocument",
                newName: "IX_Training_ValidationClientDocument_IdValidationProtocol");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationCourseDocument_IdValidationClient",
                table: "Training_ValidationClientDocument",
                newName: "IX_Training_ValidationClientDocument_IdValidationClient");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationCourseDocument_IdDocumentSerialNumber",
                table: "Training_ValidationClientDocument",
                newName: "IX_Training_ValidationClientDocument_IdDocumentSerialNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Training_ValidationClientDocument",
                table: "Training_ValidationClientDocument",
                column: "IdValidationClientDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClientDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ValidationClientDocument",
                column: "IdDocumentSerialNumber",
                principalTable: "Request_DocumentSerialNumber",
                principalColumn: "IdDocumentSerialNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationClient_IdValidationClient",
                table: "Training_ValidationClientDocument",
                column: "IdValidationClient",
                principalTable: "Training_ValidationClient",
                principalColumn: "IdValidationClient",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationProtocol_IdValidationProtocol",
                table: "Training_ValidationClientDocument",
                column: "IdValidationProtocol",
                principalTable: "Training_ValidationProtocol",
                principalColumn: "IdValidationProtocol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClientDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationClient_IdValidationClient",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClientDocument_Training_ValidationProtocol_IdValidationProtocol",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Training_ValidationClientDocument",
                table: "Training_ValidationClientDocument");

            migrationBuilder.RenameTable(
                name: "Training_ValidationClientDocument",
                newName: "Training_ValidationCourseDocument");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationClientDocument_IdValidationProtocol",
                table: "Training_ValidationCourseDocument",
                newName: "IX_Training_ValidationCourseDocument_IdValidationProtocol");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationClientDocument_IdValidationClient",
                table: "Training_ValidationCourseDocument",
                newName: "IX_Training_ValidationCourseDocument_IdValidationClient");

            migrationBuilder.RenameIndex(
                name: "IX_Training_ValidationClientDocument_IdDocumentSerialNumber",
                table: "Training_ValidationCourseDocument",
                newName: "IX_Training_ValidationCourseDocument_IdDocumentSerialNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Training_ValidationCourseDocument",
                table: "Training_ValidationCourseDocument",
                column: "IdValidationClientDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationCourseDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ValidationCourseDocument",
                column: "IdDocumentSerialNumber",
                principalTable: "Request_DocumentSerialNumber",
                principalColumn: "IdDocumentSerialNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationCourseDocument_Training_ValidationClient_IdValidationClient",
                table: "Training_ValidationCourseDocument",
                column: "IdValidationClient",
                principalTable: "Training_ValidationClient",
                principalColumn: "IdValidationClient",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationCourseDocument_Training_ValidationProtocol_IdValidationProtocol",
                table: "Training_ValidationCourseDocument",
                column: "IdValidationProtocol",
                principalTable: "Training_ValidationProtocol",
                principalColumn: "IdValidationProtocol");
        }
    }
}
