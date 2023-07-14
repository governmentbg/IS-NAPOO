using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableCandidateProviderCIPOStructureActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderCIPOStructureActivity",
                columns: table => new
                {
                    IdCandidateProviderCIPOStructureActivity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    Management = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OrganisationInformationProcess = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InternalQualitySystem = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InformationProvisionMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TrainingDocumentation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConsultantsSelection = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MTBDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DataMaintenance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderCIPOStructureActivity", x => x.IdCandidateProviderCIPOStructureActivity);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderCIPOStructureActivity_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderCIPOStructureActivity_IdCandidate_Provider",
                table: "Candidate_ProviderCIPOStructureActivity",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderCIPOStructureActivity");
        }
    }
}
