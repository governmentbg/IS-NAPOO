using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableReportUploadedDocIDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_ReportUploadedDoc",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_ReportUploadedDoc",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_ReportUploadedDoc");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_ReportUploadedDoc");
        }
    }
}
