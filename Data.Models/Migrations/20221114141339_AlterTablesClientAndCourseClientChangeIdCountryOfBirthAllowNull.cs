using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesClientAndCourseClientChangeIdCountryOfBirthAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_ClientCourse",
                type: "int",
                nullable: true,
                comment: "Месторождение (държава)",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Месторождение (държава)");

            migrationBuilder.AlterColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_Client",
                type: "int",
                nullable: true,
                comment: "Месторождение (държава)",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Месторождение (държава)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_ClientCourse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Месторождение (държава)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Месторождение (държава)");

            migrationBuilder.AlterColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_Client",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Месторождение (държава)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Месторождение (държава)");
        }
    }
}
