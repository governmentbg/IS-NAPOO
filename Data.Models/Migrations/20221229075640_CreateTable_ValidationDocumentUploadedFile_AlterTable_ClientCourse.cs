using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ValidationDocumentUploadedFile_AlterTable_ClientCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Training_ClientCourse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Адрес");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Training_ClientCourse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "E-mail адрес");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Training_ClientCourse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Телефон");

            migrationBuilder.CreateTable(
                name: "Training_ValidationDocumentUploadedFile",
                columns: table => new
                {
                    IdValidationDocumentUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClientDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с издадени документи на курсисти"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationDocumentUploadedFile", x => x.IdValidationDocumentUploadedFile);
                    table.ForeignKey(
                        name: "FK_Training_ValidationDocumentUploadedFile_Training_ValidationClientDocument_IdValidationClientDocument",
                        column: x => x.IdValidationClientDocument,
                        principalTable: "Training_ValidationClientDocument",
                        principalColumn: "IdValidationClientDocument",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Прикачени файлове за документи на курсисти за процедура по валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationDocumentUploadedFile_IdValidationClientDocument",
                table: "Training_ValidationDocumentUploadedFile",
                column: "IdValidationClientDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationDocumentUploadedFile");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Training_ClientCourse");
        }
    }
}
