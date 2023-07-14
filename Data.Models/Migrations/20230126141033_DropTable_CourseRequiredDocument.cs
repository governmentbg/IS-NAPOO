using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class DropTable_CourseRequiredDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseRequiredDocument");

            migrationBuilder.RenameColumn(
                name: "Desciption",
                table: "Training_ClientRequiredDocument",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Training_ClientRequiredDocument",
                newName: "Desciption");

            migrationBuilder.CreateTable(
                name: "Training_CourseRequiredDocument",
                columns: table => new
                {
                    IdCourseRequiredDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: true, comment: "Връзка с обучаем от курс за обучение"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Описание"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    DocumentPrnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Фабричен номер"),
                    DocumentRegNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Регистрационен номер"),
                    IdCourseRequiredDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Тип задължителни документи за курс"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdEducation = table.Column<int>(type: "int", nullable: true, comment: "Завършено образование"),
                    IdMinimumQualificationLevel = table.Column<int>(type: "int", nullable: true, comment: "Квалификационно ниво"),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "Документа е валиден"),
                    IssueDocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на издаване на документа"),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseRequiredDocument", x => x.IdCourseRequiredDocument);
                    table.ForeignKey(
                        name: "FK_Training_CourseRequiredDocument_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse");
                    table.ForeignKey(
                        name: "FK_Training_CourseRequiredDocument_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Необходими документи за курс");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseRequiredDocument_IdClientCourse",
                table: "Training_CourseRequiredDocument",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseRequiredDocument_IdCourse",
                table: "Training_CourseRequiredDocument",
                column: "IdCourse");
        }
    }
}
