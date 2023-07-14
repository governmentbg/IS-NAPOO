using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CandidateCurriculumModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Candidate_CurriculumModification",
                columns: table => new
                {
                    IdCandidateCurriculumModification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false, comment: "Връзка с лицензирана специалност"),
                    IdModificationReason = table.Column<int>(type: "int", nullable: false, comment: "Вид на причина за промяна на учебната програма"),
                    IdModificationStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на промяната на учебната програма"),
                    ValidFromDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на влизане в сила на промяната на учебната програма"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Прикачен файл с учебната програма"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_CurriculumModification", x => x.IdCandidateCurriculumModification);
                    table.ForeignKey(
                        name: "FK_Candidate_CurriculumModification_Candidate_ProviderSpeciality_IdCandidateProviderSpeciality",
                        column: x => x.IdCandidateProviderSpeciality,
                        principalTable: "Candidate_ProviderSpeciality",
                        principalColumn: "IdCandidateProviderSpeciality",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Curriculum_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                column: "IdCandidateCurriculumModification");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_CurriculumModification_IdCandidateProviderSpeciality",
                table: "Candidate_CurriculumModification",
                column: "IdCandidateProviderSpeciality");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum",
                column: "IdCandidateCurriculumModification",
                principalTable: "Candidate_CurriculumModification",
                principalColumn: "IdCandidateCurriculumModification",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Curriculum_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum");

            migrationBuilder.DropTable(
                name: "Candidate_CurriculumModification");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Curriculum_IdCandidateCurriculumModification",
                table: "Candidate_Curriculum");

            migrationBuilder.DropColumn(
                name: "IdCandidateCurriculumModification",
                table: "Candidate_Curriculum");
        }
    }
}
