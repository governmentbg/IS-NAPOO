using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourse_IdProgram_Allow_NULL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Course_Training_Program_IdProgram",
                table: "Training_Course");

            migrationBuilder.AlterColumn<int>(
                name: "IdProgram",
                table: "Training_Course",
                type: "int",
                nullable: true,
                comment: "Връзка с Програмa за обучение, предлагани от ЦПО",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Връзка с Програмa за обучение, предлагани от ЦПО");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Course_Training_Program_IdProgram",
                table: "Training_Course",
                column: "IdProgram",
                principalTable: "Training_Program",
                principalColumn: "IdProgram");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Course_Training_Program_IdProgram",
                table: "Training_Course");

            migrationBuilder.AlterColumn<int>(
                name: "IdProgram",
                table: "Training_Course",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с Програмa за обучение, предлагани от ЦПО",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Връзка с Програмa за обучение, предлагани от ЦПО");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Course_Training_Program_IdProgram",
                table: "Training_Course",
                column: "IdProgram",
                principalTable: "Training_Program",
                principalColumn: "IdProgram",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
