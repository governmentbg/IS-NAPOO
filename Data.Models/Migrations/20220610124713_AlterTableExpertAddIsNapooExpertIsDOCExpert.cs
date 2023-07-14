using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableExpertAddIsNapooExpertIsDOCExpert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDOCExpert",
                table: "ExpComm_Expert",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNapooExpert",
                table: "ExpComm_Expert",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDOCExpert",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IsNapooExpert",
                table: "ExpComm_Expert");
        }
    }
}
