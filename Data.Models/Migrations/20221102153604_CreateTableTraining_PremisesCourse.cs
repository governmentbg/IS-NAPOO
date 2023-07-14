using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableTraining_PremisesCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_PremisesCourse",
                columns: table => new
                {
                    IdPremisesCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPremises = table.Column<int>(type: "int", nullable: false),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    IdТraininType = table.Column<int>(type: "int", nullable: true, comment: "Вид обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_PremisesCourse", x => x.IdPremisesCourse);
                    table.ForeignKey(
                        name: "FK_Training_PremisesCourse_Candidate_ProviderPremises_IdPremises",
                        column: x => x.IdPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_PremisesCourse_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Връзка между MTB и курс");

            migrationBuilder.CreateIndex(
                name: "IX_Training_PremisesCourse_IdCourse",
                table: "Training_PremisesCourse",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_PremisesCourse_IdPremises",
                table: "Training_PremisesCourse",
                column: "IdPremises");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_PremisesCourse");
        }
    }
}
