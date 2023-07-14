using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCourseRequiredDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseRequiredDocument",
                columns: table => new
                {
                    IdCourseRequiredDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    IdCourseRequiredDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Тип задължителни документи за курс"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    Desciption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Описание"),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "Документа е валиден"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseRequiredDocument", x => x.IdCourseRequiredDocument);
                    table.ForeignKey(
                        name: "FK_Training_CourseRequiredDocument_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Необходими документи за курс");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseRequiredDocument_IdCourse",
                table: "Training_CourseRequiredDocument",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseRequiredDocument");
        }
    }
}
