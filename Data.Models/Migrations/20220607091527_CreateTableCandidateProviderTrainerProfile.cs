using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderTrainerProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainerProfile",
                columns: table => new
                {
                    IdCandidateProviderTrainerProfile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false),
                    IsProfessionalDirectionQualified = table.Column<bool>(type: "bit", nullable: false),
                    IsTheory = table.Column<bool>(type: "bit", nullable: false),
                    IsPractice = table.Column<bool>(type: "bit", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderTrainerProfile", x => x.IdCandidateProviderTrainerProfile);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerProfile_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerProfile_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerProfile_IdCandidateProviderTrainer",
                table: "Candidate_ProviderTrainerProfile",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerProfile_IdProfessionalDirection",
                table: "Candidate_ProviderTrainerProfile",
                column: "IdProfessionalDirection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainerProfile");
        }
    }
}
