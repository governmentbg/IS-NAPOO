using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingProgramColumnIsServiceNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsService",
                table: "Training_Program",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Дали записът е служебен(Създаден заради разлика в рамкова програма)",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Дали записът е служебен(Създаден заради разлика в рамкова програма)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsService",
                table: "Training_Program",
                type: "bit",
                nullable: true,
                comment: "Дали записът е служебен(Създаден заради разлика в рамкова програма)",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Дали записът е служебен(Създаден заради разлика в рамкова програма)");
        }
    }
}
