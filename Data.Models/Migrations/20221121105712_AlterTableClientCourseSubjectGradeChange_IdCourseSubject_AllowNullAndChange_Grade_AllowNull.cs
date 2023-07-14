using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseSubjectGradeChange_IdCourseSubject_AllowNullAndChange_Grade_AllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Grade",
                table: "Training_ClientCourseSubjectGrade",
                type: "float",
                nullable: true,
                comment: "Обща оценка по предмет от учебен план",
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Обща оценка по предмет от учебен план");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Grade",
                table: "Training_ClientCourseSubjectGrade",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                comment: "Обща оценка по предмет от учебен план",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldComment: "Обща оценка по предмет от учебен план");
        }
    }
}
