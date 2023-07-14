using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateCandidateProviderTrainerChecking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainerChecking",
                columns: table => new
                {
                    IdCandidateProviderTrainerChecking = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Преподавател"),
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
                    table.PrimaryKey("PK_Candidate_ProviderTrainerChecking", x => x.IdCandidateProviderTrainerChecking);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerChecking_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerChecking_IdCandidateProviderTrainer",
                table: "Candidate_ProviderTrainerChecking",
                column: "IdCandidateProviderTrainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainerChecking");
        }
    }
}
