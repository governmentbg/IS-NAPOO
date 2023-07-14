using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderTrainerSpecialityAddIdUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsage",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Вид на провежданото обучение");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsage",
                table: "Candidate_ProviderTrainerSpeciality");
        }
    }
}
