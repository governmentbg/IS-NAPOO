using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableCandidate_ProviderStructureActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderStructureActivity",
                columns: table => new
                {
                    IdCandidateProviderStructureActivity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    Management = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OrganisationTrainingProcess = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CompletionCertificationTraining = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    InternalQualitySystem = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    InformationProvisionMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TrainingDocumentation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TeachersSelection = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    MTBDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DataMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderStructureActivity");
        }
    }
}
