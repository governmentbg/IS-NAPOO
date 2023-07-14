using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableTraining_CourseDocumentUploadedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseDocumentUploadedFile",
                columns: table => new
                {
                    IdCourseDocumentUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourseDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с издадени документи на курсисти"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseDocumentUploadedFile", x => x.IdCourseDocumentUploadedFile);
                    table.ForeignKey(
                        name: "FK_Training_CourseDocumentUploadedFile_Training_ClientCourseDocument_IdClientCourseDocument",
                        column: x => x.IdClientCourseDocument,
                        principalTable: "Training_ClientCourseDocument",
                        principalColumn: "IdClientCourseDocument",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Прикачени файлове за документи на курсисти");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseDocumentUploadedFile_IdClientCourseDocument",
                table: "Training_CourseDocumentUploadedFile",
                column: "IdClientCourseDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseDocumentUploadedFile");
        }
    }
}
