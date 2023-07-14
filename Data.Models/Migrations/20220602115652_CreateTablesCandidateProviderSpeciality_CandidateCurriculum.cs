using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTablesCandidateProviderSpeciality_CandidateCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderSpeciality",
                columns: table => new
                {
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderSpeciality", x => x.IdCandidateProviderSpeciality);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderSpeciality_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidate_Curriculum",
                columns: table => new
                {
                    IdCandidateCurriculum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Theory = table.Column<double>(type: "float", nullable: true),
                    Practice = table.Column<double>(type: "float", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Curriculum", x => x.IdCandidateCurriculum);
                    table.ForeignKey(
                        name: "FK_Candidate_Curriculum_Candidate_ProviderSpeciality_IdCandidateProviderSpeciality",
                        column: x => x.IdCandidateProviderSpeciality,
                        principalTable: "Candidate_ProviderSpeciality",
                        principalColumn: "IdCandidateProviderSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Curriculum_IdCandidateProviderSpeciality",
                table: "Candidate_Curriculum",
                column: "IdCandidateProviderSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderSpeciality_IdCandidate_Provider",
                table: "Candidate_ProviderSpeciality",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderSpeciality_IdSpeciality",
                table: "Candidate_ProviderSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_Curriculum");

            migrationBuilder.DropTable(
                name: "Candidate_ProviderSpeciality");
        }
    }
}
