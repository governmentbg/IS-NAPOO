using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_EventLog_Add_CurrentMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentMenu",
                table: "EventLog",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Наименование на менюто");

            migrationBuilder.AddColumn<string>(
                name: "CurrentUrl",
                table: "EventLog",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Текущ адрес на страницата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentMenu",
                table: "EventLog");

            migrationBuilder.DropColumn(
                name: "CurrentUrl",
                table: "EventLog");
        }
    }
}
