using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderTrainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainer",
                columns: table => new
                {
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdIndentType = table.Column<int>(type: "int", nullable: true),
                    Indent = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSex = table.Column<int>(type: "int", nullable: true),
                    IdNationality = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdEducation = table.Column<int>(type: "int", nullable: false),
                    EducationSpecialityNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EducationCertificateNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EducationAcademicNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsAndragog = table.Column<bool>(type: "bit", nullable: false),
                    IdContractType = table.Column<int>(type: "int", nullable: true),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderTrainer", x => x.IdCandidateProviderTrainer);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainer_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainer_IdCandidate_Provider",
                table: "Candidate_ProviderTrainer",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainer");
        }
    }
}
