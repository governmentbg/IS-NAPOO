using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_Training_ClientCourseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ClientCourseStatus",
                columns: table => new
                {
                    IdClientCourseStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Получател на услугата(обучаем) връзка с курс"),
                    IdFinishedType = table.Column<int>(type: "int", nullable: false, comment: "Приключване на курс"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientCourseStatus", x => x.IdClientCourseStatus);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourseStatus_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "История на статуса на завършване на курс за обучение от курсист");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseStatus_IdClientCourse",
                table: "Training_ClientCourseStatus",
                column: "IdClientCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientCourseStatus");
        }
    }
}
