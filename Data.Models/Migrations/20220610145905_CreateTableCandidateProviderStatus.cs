using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderStatus",
                columns: table => new
                {
                    IdCandidateProviderStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderStatus", x => x.IdCandidateProviderStatus);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderStatus_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderStatus_IdCandidate_Provider",
                table: "Candidate_ProviderStatus",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderStatus");
        }
    }
}
