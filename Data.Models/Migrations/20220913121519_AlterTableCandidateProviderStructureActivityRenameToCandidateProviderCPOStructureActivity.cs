using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderStructureActivityRenameToCandidateProviderCPOStructureActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderStructureActivity");

            migrationBuilder.CreateTable(
                name: "Candidate_ProviderCPOStructureActivity",
                columns: table => new
                {
                    IdCandidateProviderCPOStructureActivity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    Management = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OrganisationTrainingProcess = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompletionCertificationTraining = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InternalQualitySystem = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InformationProvisionMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TrainingDocumentation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TeachersSelection = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MTBDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DataMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderCPOStructureActivity", x => x.IdCandidateProviderCPOStructureActivity);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderCPOStructureActivity_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderCPOStructureActivity_IdCandidate_Provider",
                table: "Candidate_ProviderCPOStructureActivity",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderCPOStructureActivity");

            migrationBuilder.CreateTable(
                name: "Candidate_ProviderStructureActivity",
                columns: table => new
                {
                    IdCandidateProviderStructureActivity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    CompletionCertificationTraining = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    InformationProvisionMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InternalQualitySystem = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MTBDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Management = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganisationTrainingProcess = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TeachersSelection = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TrainingDocumentation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderStructureActivity", x => x.IdCandidateProviderStructureActivity);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderStructureActivity_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderStructureActivity_IdCandidate_Provider",
                table: "Candidate_ProviderStructureActivity",
                column: "IdCandidate_Provider");
        }
    }
}
