using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_SelfAssessmentReport_SelfAssessmentReportStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_SelfAssessmentReport",
                columns: table => new
                {
                    IdSelfAssessmentReport = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    Year = table.Column<int>(type: "int", maxLength: 255, nullable: false, comment: "Година на доклада за самооценка"),
                    FilingDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на подавате на доклада за самооценка"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_SelfAssessmentReport", x => x.IdSelfAssessmentReport);
                    table.ForeignKey(
                        name: "FK_Arch_SelfAssessmentReport_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Arch_SelfAssessmentReportStatus",
                columns: table => new
                {
                    IdSelfAssessmentReportStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSelfAssessmentReport = table.Column<int>(type: "int", nullable: false, comment: "Връзка с доклад за самооценка"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на доклад за самооценка"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_SelfAssessmentReportStatus", x => x.IdSelfAssessmentReportStatus);
                    table.ForeignKey(
                        name: "FK_Arch_SelfAssessmentReportStatus_Arch_SelfAssessmentReport_IdSelfAssessmentReport",
                        column: x => x.IdSelfAssessmentReport,
                        principalTable: "Arch_SelfAssessmentReport",
                        principalColumn: "IdSelfAssessmentReport",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_SelfAssessmentReport_IdCandidateProvider",
                table: "Arch_SelfAssessmentReport",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_SelfAssessmentReportStatus_IdSelfAssessmentReport",
                table: "Arch_SelfAssessmentReportStatus",
                column: "IdSelfAssessmentReport");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_SelfAssessmentReportStatus");

            migrationBuilder.DropTable(
                name: "Arch_SelfAssessmentReport");
        }
    }
}
