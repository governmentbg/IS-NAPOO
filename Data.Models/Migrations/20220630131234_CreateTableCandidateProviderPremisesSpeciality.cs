using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderPremisesSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderPremisesSpeciality",
                columns: table => new
                {
                    IdCandidateProviderTrainerSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "Метериална техническа база"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Специалност")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderPremisesSpeciality", x => x.IdCandidateProviderTrainerSpeciality);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremisesSpeciality_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremisesSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesSpeciality_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesSpeciality",
                column: "IdCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesSpeciality_IdSpeciality",
                table: "Candidate_ProviderPremisesSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderPremisesSpeciality");
        }
    }
}
