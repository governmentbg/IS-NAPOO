using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_SPPOOOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ValidationOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_CourseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ConsultingDocumentUploadedFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ConsultingClientRequiredDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_LegalCapacityOrdinanceUploadedFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Procedure_ExternalExpert",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "ExpComm_ExpertDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Control_FollowUpControlUploadedFile",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ValidationOrder");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_CourseOrder");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ConsultingDocumentUploadedFile");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ConsultingClientRequiredDocument");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_Order");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_LegalCapacityOrdinanceUploadedFile");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "ExpComm_ExpertDocument");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Control_FollowUpControlUploadedFile");
        }
    }
}
