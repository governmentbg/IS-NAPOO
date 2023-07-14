using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationCompetency_ChangeColumn_CompetencyNumber_Int : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CompetencyNumber",
                table: "Training_ValidationCompetency",
                type: "int",
                nullable: false,
                comment: "Номер на компетентност",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Номер на компетентност");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompetencyNumber",
                table: "Training_ValidationCompetency",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Номер на компетентност",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Номер на компетентност");
        }
    }
}
