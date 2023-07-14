using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CrateTableClientRequiredDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ClientRequiredDocument",
                columns: table => new
                {
                    IdClientRequiredDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: true, comment: "Връзка с курс/обучаем"),
                    IdCourseRequiredDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Тип задължителни документи за курс,курсист"),
                    IdEducation = table.Column<int>(type: "int", nullable: true, comment: "Образование:KeyType код - Education"),
                    DocumentPrnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Фабричен номер"),
                    DocumentRegNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Регистрационен номер"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    DocumentOfficialDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Официална дата на документ"),
                    Desciption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Описание"),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "Документа е валиден"),
                    IsBeforeDate = table.Column<bool>(type: "bit", nullable: false, comment: "Документа e след 2007 година"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientRequiredDocument", x => x.IdClientRequiredDocument);
                    table.ForeignKey(
                        name: "FK_Training_ClientRequiredDocument_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse");
                    table.ForeignKey(
                        name: "FK_Training_ClientRequiredDocument_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Издадени документи на курсисти");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientRequiredDocument_IdClientCourse",
                table: "Training_ClientRequiredDocument",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientRequiredDocument_IdCourse",
                table: "Training_ClientRequiredDocument",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientRequiredDocument");
        }
    }
}
