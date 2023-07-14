using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableAddMigrationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "ExpComm_ExpertProfessionalDirection",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "ExpComm_ExpertProfessionalDirection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "ExpComm_ExpertExpertCommission",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "ExpComm_ExpertExpertCommission",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "ExpComm_ExpertDOC",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "ExpComm_ExpertDOC",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "ExpComm_Expert",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "ExpComm_ExpertProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "ExpComm_ExpertProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "ExpComm_ExpertExpertCommission");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "ExpComm_ExpertExpertCommission");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "ExpComm_ExpertDOC");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "ExpComm_ExpertDOC");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "ExpComm_Expert");
        }
    }
}
