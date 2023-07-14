using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CourseProtocol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseProtocol",
                columns: table => new
                {
                    IdCourseProtocol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение, предлагани от ЦПО"),
                    CourseProtocolNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Номер на протокол"),
                    CourseProtocolDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на протокол"),
                    IdCourseProtocolType = table.Column<int>(type: "int", nullable: false, comment: "Вид на протокол"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseProtocol", x => x.IdCourseProtocol);
                    table.ForeignKey(
                        name: "FK_Training_CourseProtocol_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Протокол към курс за обучение, предлагани от ЦПО");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseProtocol_IdCourse",
                table: "Training_CourseProtocol",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseProtocol");
        }
    }
}
