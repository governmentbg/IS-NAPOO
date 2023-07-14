using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Survey_ChangeColumn_AdditionalText_AllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InternalCode",
                table: "Assess_Survey",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Вътрешен код на анкетата",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Вътрешен код на анкетата");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalText",
                table: "Assess_Survey",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Допълнителен текст",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldComment: "Допълнителен текст");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InternalCode",
                table: "Assess_Survey",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Вътрешен код на анкетата",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Вътрешен код на анкетата");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalText",
                table: "Assess_Survey",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "",
                comment: "Допълнителен текст",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Допълнителен текст");
        }
    }
}
