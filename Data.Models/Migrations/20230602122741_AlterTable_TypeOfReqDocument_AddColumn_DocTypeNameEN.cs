using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_TypeOfReqDocument_AddColumn_DocTypeNameEN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocTypeNameEN",
                table: "Request_TypeOfRequestedDocument",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Наименование на документ на английски");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocTypeNameEN",
                table: "Request_TypeOfRequestedDocument");
        }
    }
}
