using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ArchCandidateProviderSpeciality_ArchCandidateCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_Candidate_ProviderSpeciality",
                columns: table => new
                {
                    IdArchCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdFrameworkProgram = table.Column<int>(type: "int", nullable: true, comment: "Рамкова програма"),
                    IdFormEducation = table.Column<int>(type: "int", nullable: true, comment: "Форма на обучение"),
                    LicenceData = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на получаване на лицензия за специалността"),
                    LicenceProtNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Номер на протокол/заповед за лицензиране на специалността"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_ProviderSpeciality", x => x.IdArchCandidateProviderSpeciality);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderSpeciality_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderSpeciality_SPPOO_FrameworkProgram_IdFrameworkProgram",
                        column: x => x.IdFrameworkProgram,
                        principalTable: "SPPOO_FrameworkProgram",
                        principalColumn: "IdFrameworkProgram");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Arch_Candidate_Curriculum",
                columns: table => new
                {
                    IdArchCandidateCurriculum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdArchCandidateProvider = table.Column<int>(type: "int", nullable: false),
                    IdCandidateCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdCandidateCurriculumModification = table.Column<int>(type: "int", nullable: true),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Theory = table.Column<double>(type: "float", nullable: true),
                    Practice = table.Column<double>(type: "float", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_Curriculum", x => x.IdArchCandidateCurriculum);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Curriculum_Arch_Candidate_ProviderSpeciality_IdArchCandidateProvider",
                        column: x => x.IdArchCandidateProvider,
                        principalTable: "Arch_Candidate_ProviderSpeciality",
                        principalColumn: "IdArchCandidateProviderSpeciality",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                        column: x => x.IdCandidateCurriculumModification,
                        principalTable: "Candidate_CurriculumModification",
                        principalColumn: "IdCandidateCurriculumModification");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Curriculum_Candidate_ProviderSpeciality_IdCandidateProviderSpeciality",
                        column: x => x.IdCandidateProviderSpeciality,
                        principalTable: "Candidate_ProviderSpeciality",
                        principalColumn: "IdCandidateProviderSpeciality",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Curriculum_IdArchCandidateProvider",
                table: "Arch_Candidate_Curriculum",
                column: "IdArchCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Curriculum_IdCandidateCurriculumModification",
                table: "Arch_Candidate_Curriculum",
                column: "IdCandidateCurriculumModification");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Curriculum_IdCandidateProviderSpeciality",
                table: "Arch_Candidate_Curriculum",
                column: "IdCandidateProviderSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderSpeciality_IdCandidate_Provider",
                table: "Arch_Candidate_ProviderSpeciality",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderSpeciality_IdFrameworkProgram",
                table: "Arch_Candidate_ProviderSpeciality",
                column: "IdFrameworkProgram");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderSpeciality_IdSpeciality",
                table: "Arch_Candidate_ProviderSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_Candidate_Curriculum");

            migrationBuilder.DropTable(
                name: "Arch_Candidate_ProviderSpeciality");
        }
    }
}
