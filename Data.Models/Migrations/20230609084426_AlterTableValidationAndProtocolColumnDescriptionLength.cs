using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationAndProtocolColumnDescriptionLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Training_ValidationProtocol",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Описание на протокол",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Описание на протокол");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Training_CourseProtocol",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Описание на протокол",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Описание на протокол");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Training_ValidationProtocol",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Описание на протокол",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Описание на протокол");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Training_CourseProtocol",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Описание на протокол",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Описание на протокол");
        }
    }
}
