using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ValidationTrainer_ValidationPremises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationPremises",
                columns: table => new
                {
                    IdValidationPremises = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPremises = table.Column<int>(type: "int", nullable: false),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за валидиране"),
                    IdТrainingType = table.Column<int>(type: "int", nullable: true, comment: "Вид обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationPremises", x => x.IdValidationPremises);
                    table.ForeignKey(
                        name: "FK_Training_ValidationPremises_Candidate_ProviderPremises_IdPremises",
                        column: x => x.IdPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_ValidationPremises_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Връзка между MTB и курс за валидиране");

            migrationBuilder.CreateTable(
                name: "Training_ValidationTrainer",
                columns: table => new
                {
                    IdValidationTrainer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTrainer = table.Column<int>(type: "int", nullable: false),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за валидиране"),
                    IdТrainingType = table.Column<int>(type: "int", nullable: true, comment: "Вид обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationTrainer", x => x.IdValidationTrainer);
                    table.ForeignKey(
                        name: "FK_Training_ValidationTrainer_Candidate_ProviderTrainer_IdTrainer",
                        column: x => x.IdTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_ValidationTrainer_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Връзка между лектор и курс за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationPremises_IdPremises",
                table: "Training_ValidationPremises",
                column: "IdPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationPremises_IdValidationClient",
                table: "Training_ValidationPremises",
                column: "IdValidationClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationTrainer_IdTrainer",
                table: "Training_ValidationTrainer",
                column: "IdTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationTrainer_IdValidationClient",
                table: "Training_ValidationTrainer",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationPremises");

            migrationBuilder.DropTable(
                name: "Training_ValidationTrainer");
        }
    }
}
