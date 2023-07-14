using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddIdStartedProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStartedProcedure",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdStartedProcedure",
                table: "Candidate_Provider",
                column: "IdStartedProcedure");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Provider_Procedure_StartedProcedure_IdStartedProcedure",
                table: "Candidate_Provider",
                column: "IdStartedProcedure",
                principalTable: "Procedure_StartedProcedure",
                principalColumn: "IdStartedProcedure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Provider_Procedure_StartedProcedure_IdStartedProcedure",
                table: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Provider_IdStartedProcedure",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdStartedProcedure",
                table: "Candidate_Provider");
        }
    }
}
