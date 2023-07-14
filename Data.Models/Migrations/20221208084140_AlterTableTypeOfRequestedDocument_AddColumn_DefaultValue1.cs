using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTypeOfRequestedDocument_AddColumn_DefaultValue1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultValue1",
                table: "Request_TypeOfRequestedDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Стойност по подразбиране");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValue1",
                table: "Request_TypeOfRequestedDocument");
        }
    }
}
