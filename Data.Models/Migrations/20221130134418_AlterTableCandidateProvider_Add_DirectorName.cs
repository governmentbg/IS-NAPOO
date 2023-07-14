using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProvider_Add_DirectorName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DirectorName",
                table: "Candidate_Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Директор на ЦПО,ЦИПО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectorName",
                table: "Candidate_Provider");
        }
    }
}
