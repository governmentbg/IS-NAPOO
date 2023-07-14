using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProvider_AddColumns_DirectorNames_RemoveColumn_DirectorName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectorName",
                table: "Candidate_Provider");

            migrationBuilder.AddColumn<string>(
                name: "DirectorFamilyName",
                table: "Candidate_Provider",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Фамилия директор на ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "DirectorFirstName",
                table: "Candidate_Provider",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Име на директор на ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "DirectorSecondName",
                table: "Candidate_Provider",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Презиме на директор на ЦПО,ЦИПО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectorFamilyName",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "DirectorFirstName",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "DirectorSecondName",
                table: "Candidate_Provider");

            migrationBuilder.AddColumn<string>(
                name: "DirectorName",
                table: "Candidate_Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Директор на ЦПО,ЦИПО");
        }
    }
}
