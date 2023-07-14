using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderTrainerDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainerDocument",
                columns: table => new
                {
                    IdCandidateProviderTrainerDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false),
                    IdDocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentTitle = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderTrainerDocument", x => x.IdCandidateProviderTrainerDocument);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerDocument_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerDocument_IdCandidateProviderTrainer",
                table: "Candidate_ProviderTrainerDocument",
                column: "IdCandidateProviderTrainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainerDocument");
        }
    }
}
