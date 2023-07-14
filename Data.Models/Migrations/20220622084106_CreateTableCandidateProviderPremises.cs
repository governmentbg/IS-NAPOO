using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderPremises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderPremises",
                columns: table => new
                {
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false, comment: "CPO,CIPO - Кандидат Обучаваща институция"),
                    PremisesName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Наименование на материално-техническата база"),
                    PremisesNote = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IdLocation = table.Column<int>(type: "int", nullable: true, comment: "Населено място"),
                    ProviderAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Адрес"),
                    ZipCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false, comment: "Пощенски код"),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Телефон"),
                    IdOwnership = table.Column<int>(type: "int", nullable: false, comment: "Форма на собственост"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderPremises", x => x.IdCandidateProviderPremises);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremises_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremises_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremises_IdCandidate_Provider",
                table: "Candidate_ProviderPremises",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremises_IdLocation",
                table: "Candidate_ProviderPremises",
                column: "IdLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderPremises");
        }
    }
}
