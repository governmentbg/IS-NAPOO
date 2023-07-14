using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableStartedProcedureRemoveProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedure_StartedProcedure_Provider_IdProvider",
                table: "Procedure_StartedProcedure");

            migrationBuilder.DropIndex(
                name: "IX_Procedure_StartedProcedure_IdProvider",
                table: "Procedure_StartedProcedure");

            migrationBuilder.DropColumn(
                name: "IdProvider",
                table: "Procedure_StartedProcedure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProvider",
                table: "Procedure_StartedProcedure",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_StartedProcedure_IdProvider",
                table: "Procedure_StartedProcedure",
                column: "IdProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedure_StartedProcedure_Provider_IdProvider",
                table: "Procedure_StartedProcedure",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
