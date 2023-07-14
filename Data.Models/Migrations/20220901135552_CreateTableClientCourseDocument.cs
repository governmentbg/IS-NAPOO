using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableClientCourseDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ClientCourseDocument",
                columns: table => new
                {
                    IdClientCourseDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс/обучаем"),
                    IdDocumentType = table.Column<int>(type: "int", nullable: true, comment: "Документи за завършено обучение"),
                    FinishedYear = table.Column<int>(type: "int", nullable: true, comment: "Година на приключване"),
                    DocumentPrnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Фабричен номер"),
                    DocumentRegNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Регистрационен номер"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    DocumentProtocol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Протокол"),
                    TheoryResult = table.Column<decimal>(type: "decimal(3,2)", nullable: true, comment: "Оценка по теория"),
                    PracticeResult = table.Column<decimal>(type: "decimal(3,2)", nullable: true, comment: "Оценка по практика"),
                    QualificationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Правоспособност"),
                    QualificationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Степен"),
                    DocumentSerNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Сериен номер"),
                    IdDocumentStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientCourseDocument", x => x.IdClientCourseDocument);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourseDocument_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Издадени документи на курсисти");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocument_IdClientCourse",
                table: "Training_ClientCourseDocument",
                column: "IdClientCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientCourseDocument");
        }
    }
}
