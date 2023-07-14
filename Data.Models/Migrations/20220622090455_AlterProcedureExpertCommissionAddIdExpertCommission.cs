using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterProcedureExpertCommissionAddIdExpertCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedure_ExpertCommission_ExpComm_Expert_IdExpert",
                table: "Procedure_ExpertCommission");

            migrationBuilder.DropIndex(
                name: "IX_Procedure_ExpertCommission_IdExpert",
                table: "Procedure_ExpertCommission");

            migrationBuilder.RenameColumn(
                name: "IdExpert",
                table: "Procedure_ExpertCommission",
                newName: "IdExpertCommission");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdExpertCommission",
                table: "Procedure_ExpertCommission",
                newName: "IdExpert");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExpertCommission_IdExpert",
                table: "Procedure_ExpertCommission",
                column: "IdExpert");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedure_ExpertCommission_ExpComm_Expert_IdExpert",
                table: "Procedure_ExpertCommission",
                column: "IdExpert",
                principalTable: "ExpComm_Expert",
                principalColumn: "IdExpert",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
