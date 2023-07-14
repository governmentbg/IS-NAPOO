using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesNAPOORequestDocAdd_IsNotificationSent_CandidateProviderPersonAdd_IsAllowedForNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotificationSent",
                table: "Request_NAPOORequestDoc",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Изпратено известие към ЦПО");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowedForNotification",
                table: "Candidate_ProviderPerson",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Използва се при изпращането на официални съобщения към ЦПО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotificationSent",
                table: "Request_NAPOORequestDoc");

            migrationBuilder.DropColumn(
                name: "IsAllowedForNotification",
                table: "Candidate_ProviderPerson");
        }
    }
}
