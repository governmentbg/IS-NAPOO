using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingProgramColumnIdMinimumLevelEducationNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdMinimumLevelEducation",
                table: "Training_Program",
                type: "int",
                nullable: true,
                comment: "Минимално образователно равнище от рамкова програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Минимално образователно равнище от рамкова програма");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdMinimumLevelEducation",
                table: "Training_Program",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Минимално образователно равнище от рамкова програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Минимално образователно равнище от рамкова програма");
        }
    }
}
