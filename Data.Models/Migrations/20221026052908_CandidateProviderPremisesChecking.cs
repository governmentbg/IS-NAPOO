using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CandidateProviderPremisesChecking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderPremisesChecking",
                columns: table => new
                {
                    IdCandidateProviderPremisesChecking = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "Връзка с MTB"),
                    CheckDone = table.Column<bool>(type: "bit", nullable: false, comment: "Извършена проверка от експерт на НАПОО"),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Коментар"),
                    CheckingDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на проверка"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderPremisesChecking", x => x.IdCandidateProviderPremisesChecking);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremisesChecking_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesChecking_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesChecking",
                column: "IdCandidateProviderPremises");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderPremisesChecking");
        }
    }
}
