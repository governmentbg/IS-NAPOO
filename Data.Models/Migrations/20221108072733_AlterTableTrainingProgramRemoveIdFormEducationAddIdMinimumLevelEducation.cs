using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingProgramRemoveIdFormEducationAddIdMinimumLevelEducation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFormEducation",
                table: "Training_Program");

            migrationBuilder.AddColumn<int>(
                name: "IdMinimumLevelEducation",
                table: "Training_Program",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Минимално образователно равнище от рамкова програма");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdMinimumLevelEducation",
                table: "Training_Program");

            migrationBuilder.AddColumn<int>(
                name: "IdFormEducation",
                table: "Training_Program",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Форма на обучение");
        }
    }
}
