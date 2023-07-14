using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderTrainerAdd_DiplomaNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiplomaNumber",
                table: "Candidate_ProviderTrainer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Номер на диплома");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiplomaNumber",
                table: "Candidate_ProviderTrainer");
        }
    }
}
