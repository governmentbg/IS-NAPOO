using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ValidationClientRequiredDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationClientRequiredDocument",
                columns: table => new
                {
                    IdValidationClientRequiredDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за валидиране"),
                    IdCourseRequiredDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Тип задължителни документи за курс,курсист"),
                    IdEducation = table.Column<int>(type: "int", nullable: true, comment: "Образование:KeyType код - Education"),
                    DocumentPrnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Фабричен номер"),
                    DocumentRegNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Регистрационен номер"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    DocumentOfficialDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Официална дата на документ"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Описание"),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "Документа е валиден"),
                    IsBeforeDate = table.Column<bool>(type: "bit", nullable: false, comment: "Документа e след 2007 година"),
                    IdMinimumQualificationLevel = table.Column<int>(type: "int", nullable: true, comment: "Квалификационно ниво"),
                    IssueDocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на издаване на документа"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationClientRequiredDocument", x => x.IdValidationClientRequiredDocument);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientRequiredDocument_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Издадени документи на курсисти за курс по валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientRequiredDocument_IdValidationClient",
                table: "Training_ValidationClientRequiredDocument",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationClientRequiredDocument");
        }
    }
}
