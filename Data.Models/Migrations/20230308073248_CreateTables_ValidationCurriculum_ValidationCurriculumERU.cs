using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ValidationCurriculum_ValidationCurriculumERU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationCurriculum",
                columns: table => new
                {
                    IdValidationCurriculum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                    table.PrimaryKey("PK_Training_ValidationCurriculum", x => x.IdValidationCurriculum);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCurriculum_Candidate_ProviderSpeciality_IdCandidateProviderSpeciality",
                        column: x => x.IdCandidateProviderSpeciality,
                        principalTable: "Candidate_ProviderSpeciality",
                        principalColumn: "IdCandidateProviderSpeciality",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCurriculum_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Training_ValidationCurriculumERU",
                columns: table => new
                {
                    IdValidationCurriculumERU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdERU = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationCurriculumERU", x => x.IdValidationCurriculumERU);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCurriculumERU_DOC_ERU_IdERU",
                        column: x => x.IdERU,
                        principalTable: "DOC_ERU",
                        principalColumn: "IdERU",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCurriculumERU_Training_ValidationCurriculum_IdValidationCurriculum",
                        column: x => x.IdValidationCurriculum,
                        principalTable: "Training_ValidationCurriculum",
                        principalColumn: "IdValidationCurriculum",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCurriculum_IdCandidateProviderSpeciality",
                table: "Training_ValidationCurriculum",
                column: "IdCandidateProviderSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCurriculum_IdValidationClient",
                table: "Training_ValidationCurriculum",
                column: "IdValidationClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCurriculumERU_IdERU",
                table: "Training_ValidationCurriculumERU",
                column: "IdERU");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCurriculumERU_IdValidationCurriculum",
                table: "Training_ValidationCurriculumERU",
                column: "IdValidationCurriculum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationCurriculumERU");

            migrationBuilder.DropTable(
                name: "Training_ValidationCurriculum");
        }
    }
}
