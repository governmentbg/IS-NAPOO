using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesProcedureDocumentAddExpert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdExpert",
                table: "Procedure_Document",
                type: "int",
                nullable: true,
                comment: "Връзка на документа с  Експерт");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_Document_IdExpert",
                table: "Procedure_Document",
                column: "IdExpert");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedure_Document_ExpComm_Expert_IdExpert",
                table: "Procedure_Document",
                column: "IdExpert",
                principalTable: "ExpComm_Expert",
                principalColumn: "IdExpert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedure_Document_ExpComm_Expert_IdExpert",
                table: "Procedure_Document");

            migrationBuilder.DropIndex(
                name: "IX_Procedure_Document_IdExpert",
                table: "Procedure_Document");

            migrationBuilder.DropColumn(
                name: "IdExpert",
                table: "Procedure_Document");
        }
    }
}
