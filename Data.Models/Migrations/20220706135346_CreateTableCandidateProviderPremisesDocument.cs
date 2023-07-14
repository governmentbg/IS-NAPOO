using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderPremisesDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderPremisesDocument",
                columns: table => new
                {
                    IdCandidateProviderPremisesDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "Метериална техническа база"),
                    IdDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Вид на документа"),
                    DocumentTitle = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Описание на документа"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderPremisesDocument", x => x.IdCandidateProviderPremisesDocument);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremisesDocument_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesDocument_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesDocument",
                column: "IdCandidateProviderPremises");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderPremisesDocument");
        }
    }
}
