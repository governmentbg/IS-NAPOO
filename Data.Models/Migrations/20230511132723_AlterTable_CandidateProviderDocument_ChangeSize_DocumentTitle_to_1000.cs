using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CandidateProviderDocument_ChangeSize_DocumentTitle_to_1000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentTitle",
                table: "Candidate_ProviderDocument",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Описание на документа",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "Описание на документа");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentTitle",
                table: "Candidate_ProviderDocument",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Описание на документа",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Описание на документа");
        }
    }
}
