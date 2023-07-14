using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureExternalExpertAddForeignKeyProcedureDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProcedureDocument",
                table: "Procedure_ExternalExpert",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExternalExpert_IdProcedureDocument",
                table: "Procedure_ExternalExpert",
                column: "IdProcedureDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedure_ExternalExpert_Procedure_Document_IdProcedureDocument",
                table: "Procedure_ExternalExpert",
                column: "IdProcedureDocument",
                principalTable: "Procedure_Document",
                principalColumn: "IdProcedureDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedure_ExternalExpert_Procedure_Document_IdProcedureDocument",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropIndex(
                name: "IX_Procedure_ExternalExpert_IdProcedureDocument",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "IdProcedureDocument",
                table: "Procedure_ExternalExpert");
        }
    }
}
