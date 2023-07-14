using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_ExpertExpertCommission_ExpertDOC_add_Comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ExpComm_ExpertExpertCommission",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "История на промяната");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ExpComm_ExpertDOC",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "История на промяната");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ExpComm_ExpertExpertCommission");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ExpComm_ExpertDOC");
        }
    }
}
