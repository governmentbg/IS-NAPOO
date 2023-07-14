using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableArchTrainerQialificatgionColumnTrainerAndQualificationNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainerQualification",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                column: "IdCandidateProviderTrainer",
                principalTable: "Candidate_ProviderTrainer",
                principalColumn: "IdCandidateProviderTrainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainerQualification",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                column: "IdCandidateProviderTrainer",
                principalTable: "Candidate_ProviderTrainer",
                principalColumn: "IdCandidateProviderTrainer",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
