using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_UserAnswer_ChangeColumn_IdAnswer_AllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer");

            migrationBuilder.AlterColumn<int>(
                name: "IdAnswer",
                table: "Assess_UserAnswer",
                type: "int",
                nullable: true,
                comment: "Връзка с  отговор",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Връзка с  отговор");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer",
                column: "IdAnswer",
                principalTable: "Assess_Answer",
                principalColumn: "IdAnswer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer");

            migrationBuilder.AlterColumn<int>(
                name: "IdAnswer",
                table: "Assess_UserAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с  отговор",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Връзка с  отговор");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_UserAnswer_Assess_Answer_IdAnswer",
                table: "Assess_UserAnswer",
                column: "IdAnswer",
                principalTable: "Assess_Answer",
                principalColumn: "IdAnswer",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
