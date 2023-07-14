using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableDOCAddIsDOI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDOI",
                table: "DOC_DOC",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Държавни образователни изисквания");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDOI",
                table: "DOC_DOC");
        }
    }
}
