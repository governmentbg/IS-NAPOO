using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesSpecialityAlterPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdCandidateProviderTrainerSpeciality",
                table: "Candidate_ProviderPremisesSpeciality",
                newName: "IdCandidateProviderPremisesSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdCandidateProviderPremisesSpeciality",
                table: "Candidate_ProviderPremisesSpeciality",
                newName: "IdCandidateProviderTrainerSpeciality");
        }
    }
}
