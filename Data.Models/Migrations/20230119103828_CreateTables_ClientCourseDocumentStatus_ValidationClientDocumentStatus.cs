using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ClientCourseDocumentStatus_ValidationClientDocumentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ClientCourseDocumentStatus",
                columns: table => new
                {
                    IdClientCourseDocumentStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourseDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с издадени документ на курсист"),
                    IdClientDocumentStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на документа за завършване на курсист"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientCourseDocumentStatus", x => x.IdClientCourseDocumentStatus);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourseDocumentStatus_Training_ClientCourseDocument_IdClientCourseDocument",
                        column: x => x.IdClientCourseDocument,
                        principalTable: "Training_ClientCourseDocument",
                        principalColumn: "IdClientCourseDocument",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "История на статуса на документ за завършване на курсист");

            migrationBuilder.CreateTable(
                name: "Training_ValidationClientDocumentStatus",
                columns: table => new
                {
                    IdValidationClientDocumentStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClientDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с издаден документ на курсист за валидиране"),
                    IdClientDocumentStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на документа за завършване на курсист"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationClientDocumentStatus", x => x.IdValidationClientDocumentStatus);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientDocumentStatus_Training_ValidationClientDocument_IdValidationClientDocument",
                        column: x => x.IdValidationClientDocument,
                        principalTable: "Training_ValidationClientDocument",
                        principalColumn: "IdValidationClientDocument",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "История на статуса на документ за завършване на курсист за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocumentStatus_IdClientCourseDocument",
                table: "Training_ClientCourseDocumentStatus",
                column: "IdClientCourseDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientDocumentStatus_IdValidationClientDocument",
                table: "Training_ValidationClientDocumentStatus",
                column: "IdValidationClientDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientCourseDocumentStatus");

            migrationBuilder.DropTable(
                name: "Training_ValidationClientDocumentStatus");
        }
    }
}
