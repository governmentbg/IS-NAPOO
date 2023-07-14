using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderTrainerSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderTrainerSpeciality",
                columns: table => new
                {
                    IdCandidateProviderTrainerSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Преподавател"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Специалност")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderTrainerSpeciality", x => x.IdCandidateProviderTrainerSpeciality);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerSpeciality_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderTrainerSpeciality_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerSpeciality_IdCandidateProviderTrainer",
                table: "Candidate_ProviderTrainerSpeciality",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerSpeciality_IdSpeciality",
                table: "Candidate_ProviderTrainerSpeciality",
                column: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderTrainerSpeciality");
        }
    }
}
