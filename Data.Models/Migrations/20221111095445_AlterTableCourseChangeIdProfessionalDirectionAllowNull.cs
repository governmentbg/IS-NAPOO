using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseChangeIdProfessionalDirectionAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Client_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_Client");

            migrationBuilder.AlterColumn<int>(
                name: "IdProfessionalDirection",
                table: "Training_Client",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Client_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_Client",
                column: "IdProfessionalDirection",
                principalTable: "SPPOO_ProfessionalDirection",
                principalColumn: "IdProfessionalDirection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_Client_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_Client");

            migrationBuilder.AlterColumn<int>(
                name: "IdProfessionalDirection",
                table: "Training_Client",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Client_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_Client",
                column: "IdProfessionalDirection",
                principalTable: "SPPOO_ProfessionalDirection",
                principalColumn: "IdProfessionalDirection",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
