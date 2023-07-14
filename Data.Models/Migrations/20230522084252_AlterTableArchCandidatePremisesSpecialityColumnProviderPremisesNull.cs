using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableArchCandidatePremisesSpecialityColumnProviderPremisesNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: true,
                comment: "Метериална техническа база",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Метериална техническа база");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                column: "IdCandidateProviderPremises",
                principalTable: "Candidate_ProviderPremises",
                principalColumn: "IdCandidateProviderPremises");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Метериална техническа база",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Метериална техническа база");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                column: "IdCandidateProviderPremises",
                principalTable: "Candidate_ProviderPremises",
                principalColumn: "IdCandidateProviderPremises",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
