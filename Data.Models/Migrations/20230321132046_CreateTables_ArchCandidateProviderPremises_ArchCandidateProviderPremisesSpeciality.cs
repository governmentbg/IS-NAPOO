using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ArchCandidateProviderPremises_ArchCandidateProviderPremisesSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_Candidate_ProviderPremises",
                columns: table => new
                {
                    IdArchCandidateProviderPremises = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false),
                    IdArchCandidateProvider = table.Column<int>(type: "int", nullable: false),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false, comment: "CPO,CIPO - Кандидат Обучаваща институция"),
                    PremisesName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Наименование на материално-техническата база"),
                    PremisesNote = table.Column<string>(type: "ntext", nullable: true, comment: "Кратко описание"),
                    IdLocation = table.Column<int>(type: "int", nullable: true, comment: "Населено място"),
                    ProviderAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Адрес"),
                    ZipCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false, comment: "Пощенски код"),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Телефон"),
                    IdOwnership = table.Column<int>(type: "int", nullable: false, comment: "Форма на собственост"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    InactiveDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на деактивиране на базата"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_ProviderPremises", x => x.IdArchCandidateProviderPremises);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremises_Arch_Candidate_Provider_IdArchCandidateProvider",
                        column: x => x.IdArchCandidateProvider,
                        principalTable: "Arch_Candidate_Provider",
                        principalColumn: "IdArchCandidateProvider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremises_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremises_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                });

            migrationBuilder.CreateTable(
                name: "Arch_Candidate_ProviderPremisesSpeciality",
                columns: table => new
                {
                    IdArchCandidateProviderPremisesSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremisesSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdArchCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "АРХИВ - Метериална техническа база"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "Метериална техническа база"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Специалност"),
                    IdUsage = table.Column<int>(type: "int", nullable: false, comment: "Вид на провежданото обучение"),
                    IdComplianceDOC = table.Column<int>(type: "int", nullable: false, comment: "Съответствие с ДОС"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_ProviderPremisesSpeciality", x => x.IdArchCandidateProviderPremisesSpeciality);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Arch_Candidate_ProviderPremises_IdArchCandidateProviderPremises",
                        column: x => x.IdArchCandidateProviderPremises,
                        principalTable: "Arch_Candidate_ProviderPremises",
                        principalColumn: "IdArchCandidateProviderPremises",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderPremisesSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremises_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderPremises",
                column: "IdArchCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremises_IdCandidate_Provider",
                table: "Arch_Candidate_ProviderPremises",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremises_IdLocation",
                table: "Arch_Candidate_ProviderPremises",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremisesSpeciality_IdArchCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                column: "IdArchCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremisesSpeciality_IdCandidateProviderPremises",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                column: "IdCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderPremisesSpeciality_IdSpeciality",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_Candidate_ProviderPremisesSpeciality");

            migrationBuilder.DropTable(
                name: "Arch_Candidate_ProviderPremises");
        }
    }
}
