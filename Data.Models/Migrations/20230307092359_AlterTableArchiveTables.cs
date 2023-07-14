using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableArchiveTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_Provider_Candidate_Provider_IdCandidate_Provider",
                table: "Arch_Candidate_Provider");

            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_Provider_Region_IdRegionAdmin",
                table: "Arch_Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Arch_Candidate_Provider_IdCandidate_Provider",
                table: "Arch_Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Arch_Candidate_Provider_IdRegionAdmin",
                table: "Arch_Candidate_Provider");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Връзка с Преподавател");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: false,
                comment: "Връзка с Преподавател",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdCandidate_Provider",
                table: "Arch_Candidate_Provider",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdRegionAdmin",
                table: "Arch_Candidate_Provider",
                column: "IdRegionAdmin");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_Provider_Candidate_Provider_IdCandidate_Provider",
                table: "Arch_Candidate_Provider",
                column: "IdCandidate_Provider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_Provider_Region_IdRegionAdmin",
                table: "Arch_Candidate_Provider",
                column: "IdRegionAdmin",
                principalTable: "Region",
                principalColumn: "idRegion");
        }
    }
}
