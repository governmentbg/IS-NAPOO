using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateCandidateProviderTrainerQualification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainerQualification",
                columns: table => new
                {
                    IdCandidateProviderTrainerQualification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false),
                    QualificationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdQualificationType = table.Column<int>(type: "int", nullable: false),
                    IdProfession = table.Column<int>(type: "int", nullable: true),
                    IdTrainingQualificationType = table.Column<int>(type: "int", nullable: false),
                    QualificationDuration = table.Column<int>(type: "int", nullable: true),
                    TrainingFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainingTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderTrainerQualification", x => x.IdCandidateProviderTrainerQualification);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerQualification_SPPOO_Profession_IdProfession",
                        column: x => x.IdProfession,
                        principalTable: "SPPOO_Profession",
                        principalColumn: "IdProfession");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerQualification_IdCandidateProviderTrainer",
                table: "Candidate_ProviderTrainerQualification",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerQualification_IdProfession",
                table: "Candidate_ProviderTrainerQualification",
                column: "IdProfession");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainerQualification");
        }
    }
}
