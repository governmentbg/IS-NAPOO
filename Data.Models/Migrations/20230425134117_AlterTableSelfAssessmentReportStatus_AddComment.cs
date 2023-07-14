using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableSelfAssessmentReportStatus_AddComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "EventLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "IP адрес",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "IP адрес");

            migrationBuilder.AlterColumn<string>(
                name: "BrowserInformation",
                table: "EventLog",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Браузър на потребителя",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Браузър на потребителя");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Arch_SelfAssessmentReportStatus",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Коментар при операция с доклад за самооценка");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Arch_SelfAssessmentReportStatus");

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "EventLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "IP адрес",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "IP адрес");

            migrationBuilder.AlterColumn<string>(
                name: "BrowserInformation",
                table: "EventLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "Браузър на потребителя",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "Браузър на потребителя");
        }
    }
}
