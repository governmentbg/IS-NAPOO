using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingCourseCommissionChangeNameToTrainingCourseCommissionMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseCommission");

            migrationBuilder.CreateTable(
                name: "Training_CourseCommissionMember",
                columns: table => new
                {
                    IdCourseCommissionMember = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IsChairman = table.Column<bool>(type: "bit", nullable: false, comment: "Дали е председател на комисия"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курса за обучение, предлаган от ЦПО"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseCommissionMember", x => x.IdCourseCommissionMember);
                    table.ForeignKey(
                        name: "FK_Training_CourseCommissionMember_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Член на изпитна комисия към курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCommissionMember_IdCourse",
                table: "Training_CourseCommissionMember",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseCommissionMember");

            migrationBuilder.CreateTable(
                name: "Training_CourseCommission",
                columns: table => new
                {
                    IdCourseCommission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курса за обучение, предлаган от ЦПО"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    IsChairman = table.Column<bool>(type: "bit", nullable: false, comment: "Дали е председател на комисия"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseCommission", x => x.IdCourseCommission);
                    table.ForeignKey(
                        name: "FK_Training_CourseCommission_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Членове на изпитна комисия към курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCommission_IdCourse",
                table: "Training_CourseCommission",
                column: "IdCourse");
        }
    }
}
