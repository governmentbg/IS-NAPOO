using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Program_ChangeColumn_ProgramNumber_AllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProgramNumber",
                table: "Training_Program",
                type: "int",
                nullable: true,
                comment: "Номер на програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Номер на програма");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProgramNumber",
                table: "Training_Program",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Номер на програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Номер на програма");
        }
    }
}
