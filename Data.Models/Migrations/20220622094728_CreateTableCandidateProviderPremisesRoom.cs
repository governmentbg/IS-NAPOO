using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderPremisesRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderPremisesRoom",
                columns: table => new
                {
                    IdCandidateProviderPremisesRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false, comment: "Метериална техническа база"),
                    PremisesRoomName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Наименование на помещението"),
                    Equipment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IdUsage = table.Column<int>(type: "int", nullable: false, comment: "Вид на провежданото обучение"),
                    IdPremisesType = table.Column<int>(type: "int", nullable: false, comment: "Вид на помещението"),
                    Area = table.Column<int>(type: "int", nullable: true, comment: "Приблизителна площ (кв. м."),
                    Workplace = table.Column<int>(type: "int", nullable: true, comment: "Брой работни места"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderPremisesRoom", x => x.IdCandidateProviderPremisesRoom);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderPremisesRoom_Candidate_ProviderPremises_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesRoom_IdCandidate_Provider",
                table: "Candidate_ProviderPremisesRoom",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderPremisesRoom");
        }
    }
}
