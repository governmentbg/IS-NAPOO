using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProgramChangeIdFrameworkProgramAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Program_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Training_Program");

            migrationBuilder.AlterColumn<int>(
                name: "IdFrameworkProgram",
                table: "Training_Program",
                type: "int",
                nullable: true,
                comment: "Рамкова програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Рамкова програма");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Program_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Training_Program",
                column: "IdFrameworkProgram",
                principalTable: "SPPOO_FrameworkProgram",
                principalColumn: "IdFrameworkProgram");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Program_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Training_Program");

            migrationBuilder.AlterColumn<int>(
                name: "IdFrameworkProgram",
                table: "Training_Program",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Рамкова програма",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Рамкова програма");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Program_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Training_Program",
                column: "IdFrameworkProgram",
                principalTable: "SPPOO_FrameworkProgram",
                principalColumn: "IdFrameworkProgram",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
