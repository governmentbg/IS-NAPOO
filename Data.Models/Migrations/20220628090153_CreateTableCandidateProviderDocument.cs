using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderDocument",
                columns: table => new
                {
                    IdCandidateProviderDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    CandidateProviderIdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    IdDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Вид на документа"),
                    DocumentTitle = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Описание на документа"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "UploadedFileName"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderDocument", x => x.IdCandidateProviderDocument);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderDocument_Candidate_Provider_CandidateProviderIdCandidate_Provider",
                        column: x => x.CandidateProviderIdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderDocument_CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument",
                column: "CandidateProviderIdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderDocument");
        }
    }
}
